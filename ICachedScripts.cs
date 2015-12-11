using System.IO;

namespace ContinuousRunner
{
    public interface ICachedScripts
    {
        IScript Get(FileInfo fileInfo);
    }
}
