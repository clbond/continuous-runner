using System.IO;

namespace TestRunner
{
    using Data;

    public interface IScriptParser
    {
        SyntaxTree Parse(FileInfo fileInfo);
    }
}
