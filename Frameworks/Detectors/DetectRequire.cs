using System;
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
            if (IsRequireCall(expr, @"require"))
            {
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
                        return false;
                }
            }
            else if (IsRequireCall(expr, @"define"))
            {
                return true;
            }

            return false;
        }

        private static bool IsRequireCall(CallExpression expr, string function)
        {
            var callee = expr.Callee;

            switch (callee.Type)
            {
                case SyntaxNodes.Identifier:
                    var identifier = callee.As<Identifier>();
                    return string.Equals(identifier.Name, function, StringComparison.CurrentCulture);
                case SyntaxNodes.MemberExpression:
                    var member = callee.As<MemberExpression>();
                    if (member.Property.Type == SyntaxNodes.Identifier)
                    {
                        var property = member.Property.As<Identifier>();
                        return string.Equals(property.Name, function, StringComparison.CurrentCulture);
                    }
                    break;
            }

            return false;
        }

        #endregion
    }
}
