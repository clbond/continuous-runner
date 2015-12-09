using System.IO;

namespace ContinuousRunner
{
    using Data;

    public interface IParser
    {
        ExpressionTree Parse(FileInfo fileInfo);

        ExpressionTree Parse(string script);
    }
}
