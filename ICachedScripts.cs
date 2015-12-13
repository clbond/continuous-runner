using System;
using System.IO;

namespace ContinuousRunner
{
    public interface ICachedScripts
    {
        /// <summary>
        /// The <paramref name="load"/> argument given to this function will be used to load
        /// the script in question as well as all of its direct and indirect dependencies, so
        /// you should expect for it to be called possibly hundreds of times for each script
        /// you load. By loading everything and keeping it in-memory we can dramatically speed
        /// up each test run, at the cost of memory usage.
        /// </summary>
        IScript Get(FileInfo fileInfo, Func<FileInfo, IScript> load);

        void Remove(FileInfo fileInfo);

        void Remove(IScript script);
    }
}
