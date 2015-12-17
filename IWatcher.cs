using System;
using System.IO;

namespace ContinuousRunner
{
    public interface IWatcher
    {
        /// <summary>
        /// Begin watching <paramref name="scriptPath"/> for filesystem changes, sending changes to <seealso cref="ISourceMutator"/>.
        /// </summary>
        IDisposable Watch(DirectoryInfo scriptPath);

        /// <summary>
        /// Watch the root path specified in <seealso cref="IInstanceContext"/>.
        /// </summary>
        IDisposable Watch();
    }
}
