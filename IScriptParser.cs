using System.IO;

namespace TestRunner
{
    public interface IScriptParser
    {
        SyntaxTree Parse(FileInfo fileInfo);
    }
}
