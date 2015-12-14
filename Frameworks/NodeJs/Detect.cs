using System;
using System.Linq;

using Jint.Parser.Ast;

namespace ContinuousRunner.Frameworks.NodeJs
{
    using Extensions;

    public class Detect : IDetector<Framework>
    {
        #region Implementation of IDetector<Framework>

        public Framework Analyze(IScript script)
        {
            var matches = script.ExpressionTree.Search<CallExpression>(IsRequireCall);
            if (matches.Any(IsRequireWithStringArgument))
            {
                return Framework.NodeJs;
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

            if (expr.Arguments.Count() != 1)
            {
                return false;
            }

            var argument = expr.Arguments.First();

            switch (argument.Type)
            {
                case SyntaxNodes.ArrayExpression: // require(['foo']) is AMD syntax, not NodeJS
                    return false;
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
                    return string.Equals(identifier.Name, @"require", StringComparison.CurrentCulture);
                case SyntaxNodes.MemberExpression:
                    var member = callee.As<MemberExpression>();
                    if (member.Property.Type == SyntaxNodes.Identifier)
                    {
                        var property = member.Property.As<Identifier>();
                        return string.Equals(property.Name, @"require", StringComparison.CurrentCulture);
                    }
                    break;
            }

            return false;
        }

        #endregion
    }
}
