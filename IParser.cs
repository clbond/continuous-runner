using System.IO;

namespace ContinuousRunner
{
    using Data;

    public interface IParser
    {
        SyntaxTree Parse(FileInfo fileInfo);

        SyntaxTree Parse(string script);
    }
}
