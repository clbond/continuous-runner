using System.IO;
namespace ContinuousRunner
{
    using Data;

    public interface IParser<TNode> where TNode : class
    {
        ExpressionTree<TNode> Parse(FileInfo fileInfo);

        ExpressionTree<TNode> Parse(string script);

        ExpressionTree<TNode> TryParse(FileInfo fileInfo);

        ExpressionTree<TNode> TryParse(string script);
    }
}
