using System;
using System.IO;

namespace ContinuousRunner
{
    public interface ICachedScripts
    {
        IScript Get(FileInfo fileInfo, Func<FileInfo, IScript> load);

        void Remove(FileInfo fileInfo);

        void Remove(IScript script);
    }
}
