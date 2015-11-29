using System.Linq;

using Jint.Parser.Ast;

namespace ContinuousRunner.Frameworks.Detectors
{
    using Extensions;

    public class DetectRequire : IDetector
    {
        #region Implementation of IDetector

        public Framework Detect(IScript script)
        {
            var matches = script.SyntaxTree.Search<CallExpression>(IsRequireWithStringArgument);
            if (matches.Any())
            {
                return Framework.RequireJs;
            }

            return Framework.None;
        }

        #endregion

        #region Private methods

        private static bool IsRequireWithStringArgument(CallExpression expr)
        {
            if (!IsRequireCall(expr))
            {
                return false;
            }

            if (!expr.Arguments.Any())
            {
                return false;
            }

            // Determine whether this call to require is more likely to be a NodeJS module import, or
            // an asynchronous import (RequireJS). Both RequireJS and NodeJS use require() but they have
            // different signatures, and this function is trying to figure out which one is being invoked.
            var argument = expr.Arguments.First();

            switch (argument.Type)
            {
                case SyntaxNodes.ArrayExpression:
                    return true;
                case SyntaxNodes.Literal:
                    return true;
            }

            return false;
        }

        private static bool IsRequireCall(CallExpression expr)
        {
            var callee = expr.Callee;

            switch (callee.Type)
            {
                case SyntaxNodes.Identifier:
                    var identifier = callee.As<Identifier>();
                    return Constants.FunctionIdentifiers.SuiteFunctions.Contains(identifier.Name);
                case SyntaxNodes.MemberExpression:
                    var member = callee.As<MemberExpression>();
                    if (member.Property.Type == SyntaxNodes.Identifier)
                    {
                        var property = member.Property.As<Identifier>();
                        return Constants.FunctionIdentifiers.SuiteFunctions.Contains(property.Name);
                    }
                    break;
            }

            return false;
        }

        #endregion
    }
}
