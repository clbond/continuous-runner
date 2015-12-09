namespace ContinuousRunner.Data
{
    public class ExpressionTree
    {
        public Jint.Parser.Ast.SyntaxNode Root { get; set; }

        public ExpressionTree(Jint.Parser.Ast.SyntaxNode expr)
        {
            Root = expr;
        }
    }
}
