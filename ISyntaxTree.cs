namespace TestRunner
{
    public interface ISyntaxTree
    {
        Jint.Parser.Ast.Program Root { set; }
    }
}