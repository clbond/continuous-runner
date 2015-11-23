using System;
using System.Collections.Generic;
using System.Linq;
using ContinuousRunner.Data;
using ContinuousRunner.Extensions;
using JetBrains.Annotations;
using Jint.Parser.Ast;
using Magnum;

namespace ContinuousRunner.Impl
{
    public class SuiteReader : ISuiteReader
    {
        private readonly IResultObserver _observer;

        private readonly IResultFactory _resultFactory;

        public SuiteReader(
            [NotNull] IResultObserver observer,
            [NotNull] IResultFactory resultFactory)
        {
            Guard.AgainstNull(observer, nameof(observer));
            Guard.AgainstNull(resultFactory, nameof(resultFactory));

            _observer = observer;

            _resultFactory = resultFactory;
        }

        #region Implementation of ISuiteReader

        public IEnumerable<TestSuite> Get(IScript script)
        {
            if (Constants.SearchExpression.IsMatch(script.File.Name))
            {
                return Parse(script);
            }

            return Enumerable.Empty<TestSuite>();
        }

        #endregion

        #region Private methods

        private IEnumerable<TestSuite> Parse(IScript script)
        {
            var suites = script.SyntaxTree.Root.Search<CallExpression>(IsSuite);

            return suites.Select(s => ToSuite(script, s));
        }

        private TestSuite ToSuite(IScript script, CallExpression expr)
        {
            var tests = expr.Arguments.Search<CallExpression>(IsTest);

            var suite = new TestSuite
                        {
                            Name = GetDescription(expr)
                        };

            suite.Tests = tests.Select(t => ToTest(script, suite, t)).ToList();

            return suite;
        }

        private ITest ToTest(IScript script, TestSuite suite, CallExpression expr)
        {
            Action<TestResultChanged> changeHandler = changed => _observer.ResultChanged(changed);

            var initialState = _resultFactory.InitialState();

            var description = GetDescription(expr);

            return new Test(script, initialState, changeHandler)
                   {
                       Suite = suite,
                       Name = description,
                       Id = GetDeterministicId(suite, description)
                   };
        }

        private static Guid GetDeterministicId(TestSuite suite, string description)
        {
            var hashCode = suite.Name.GetHashCode() ^ description.GetHashCode();

            var bytes = To16Bytes(BitConverter.GetBytes(hashCode));

            return new Guid(bytes);
        }

        private static byte[] To16Bytes(byte[] bytes)
        {
            var b = new byte[16];
            bytes.CopyTo(b, b.Length - bytes.Length - 1);

            return b;
        }

        private static string GetDescription(CallExpression expr)
        {
            var firstArgument = expr.Arguments.FirstOrDefault();
            if (firstArgument?.Type == SyntaxNodes.Literal)
            {
                var literal = firstArgument.As<Literal>();

                return literal?.Value as string;
            }

            throw new TestException(@"Cannot determine suite or test name from call expression argument");
        }

        private static bool IsSuite(CallExpression expr)
        {
            return InvocationMatch(expr, Constants.FunctionIdentifiers.SuiteFunctions);
        }

        public static bool IsTest(CallExpression expr)
        {
            return InvocationMatch(expr, Constants.FunctionIdentifiers.TestFunction);
        }

        private static bool InvocationMatch(CallExpression expr, IEnumerable<string> functions)
        {
            if (expr.Callee.Type == SyntaxNodes.Identifier)
            {
                return functions.Any(f => f.Equals(expr.Callee.As<Identifier>().Name));
            }

            return false;
        }

        #endregion
    }
}