using ContinuousRunner.Data;

using Microsoft.CodeAnalysis.CSharp;

namespace ContinuousRunner
{
    public interface IClass : IProjectSource
    {
        ExpressionTree<CSharpSyntaxNode> ExpressionTree { get; }
    }
}
