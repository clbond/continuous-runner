using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jint.Parser.Ast;
using Microsoft.ClearScript.V8;

using NLog;

namespace ContinuousRunner.Impl
{
    using Data;
    using Frameworks;

    public class ScriptRunner : IScriptRunner
    {
        [Import] private readonly IFrameworkDetector _frameworkDetector;

        [Import] private readonly IParser _parser;
        
        #region Implementation of IScriptRunner

        public IEnumerable<Task<TestResult>> RunAsync(IScript script)
        {
            return script.Suites.SelectMany(s => s.Tests.Select(t => RunTestAsync(script, t)));
        }

        public IEnumerable<Task<TestResult>> RunAsync(IScript script, TestSuite suite)
        {
            return suite.Tests.Select(t => RunTestAsync(script, t));
        }

        public Task<TestResult> RunAsync(IScript script, ITest test)
        {
            return RunTestAsync(script, test);
        }

        #endregion

        #region Private methods

        private Task<TestResult> RunTestAsync(IScript script, ITest test)
        {
            var logger = LogManager.GetCurrentClassLogger();

            var completionSource = new TaskCompletionSource<TestResult>();

            // ReSharper disable once CatchAllClause
            ThreadPool.QueueUserWorkItem(
                state =>
                {
                    var engine = new V8ScriptEngine();
                    try
                    {
                        logger.Debug("Installing frameworks into V8 execution context");

                        _frameworkDetector.InstallFrameworks(script, script.Frameworks, engine);

                        logger.Info($"Executing script: {test.Name}");

                        var code = WrapInCallExpression(test.RawCode);

                        engine.Execute($"{{{code}}}");

                        completionSource.SetResult(new TestResult {Status = TestStatus.Failed, Logs = script.Logs});
                    }
                    catch (Exception ex)
                    {
                        completionSource.SetException(ex);
                    }
                    finally
                    {
                        engine.Dispose();
                    }
                });

            return completionSource.Task;
        }

        private string WrapInCallExpression(string code)
        {
            var expr = _parser.TryParse(code);
            if (expr != null)
            {
                if (expr.Root.Type == SyntaxNodes.FunctionDeclaration ||
                    expr.Root.Type == SyntaxNodes.FunctionExpression)
                {
                    return $"({code})();";
                }
            }

            return code;
        }
    }

    #endregion
}