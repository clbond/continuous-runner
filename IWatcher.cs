using System;
using System.IO;

namespace TestRunner
{
    public interface IWatcher : IDisposable
    {
        DirectoryInfo Root { get; set; }

        void Watch();

        void Stop();
    }
}
