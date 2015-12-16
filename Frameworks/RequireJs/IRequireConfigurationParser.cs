using Jint.Parser.Ast;

namespace ContinuousRunner.Frameworks.RequireJs
{
    public interface IRequireConfigurationParser
    {
        IRequireConfiguration Parse(ObjectExpression expression, SyntaxNode root);
    }
}
