namespace ContinuousRunner.Data
{
    public class SyntaxTree
    {
        public Jint.Parser.Ast.Program Root { get; set; }

        public string ToCode()
        {
            return JavaScriptGenerator.Generate(Root);
        }
    }
}
