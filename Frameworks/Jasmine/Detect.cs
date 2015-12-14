using System.Linq;

using Jint.Parser.Ast;

namespace ContinuousRunner.Frameworks.Jasmine
{
    using Extensions;

    public class Detect : IDetector<Framework>
    {
        #region Implementation of IDetector<Framework>

        public Framework Analyze(IScript script)
        {
            var matches = script.ExpressionTree.Search<CallExpression>(IsJasmineInvocation);
            if (matches.Any())
            {
                return Framework.Jasmine;
            }

            return Framework.None;
        }

        private static bool IsJasmineInvocation(CallExpression expr)
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
