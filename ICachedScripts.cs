using System.IO;

namespace ContinuousRunner
{
    public interface ICachedScripts
    {
        IScript Load(FileInfo fileInfo);

        void Remove(FileInfo fileInfo);

        void Remove(IScript script);
    }
}
