using System.IO;

namespace ContinuousRunner
{
    public interface ICachedScripts
    {
        IScript Get(FileInfo fileInfo);

        void Remove(FileInfo fileInfo);

        void Remove(IScript script);
    }
}
