using System;
using System.IO;

namespace TestRunner
{
    public interface IBackgroundWatcher : IDisposable
    {
        void Watch(DirectoryInfo path);

        void Stop();
    }
}
