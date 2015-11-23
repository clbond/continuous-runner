using System;
using System.IO;

namespace ContinuousRunner
{
    public interface IWatcher : IDisposable
    {
        DirectoryInfo Root { get; set; }

        void Watch();

        void Stop();
    }
}
