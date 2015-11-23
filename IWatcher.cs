using System;
using System.IO;

namespace ContinuousRunner
{
    public interface IWatcher
    {
        /// <summary>
        /// Begin watching <paramref name="scriptPath"/> for filesystem changes, sending changes to <seealso cref="ISourceMutator"/>.
        /// </summary>
        ICancellable Watch(DirectoryInfo scriptPath);
    }
}
