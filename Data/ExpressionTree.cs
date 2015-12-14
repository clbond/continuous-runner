namespace ContinuousRunner.Data
{
    public class ExpressionTree<TNode> where TNode : class
    {
        public TNode Root { get; set; }

        public ExpressionTree(TNode expr)
        {
            Root = expr;
        }
    }
}
