using System.IO;

namespace ContinuousRunner
{
    using Data;

    public interface IScriptParser
    {
        SyntaxTree Parse(FileInfo fileInfo);
    }
}
