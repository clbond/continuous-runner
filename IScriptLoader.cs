using System.IO;

namespace ContinuousRunner
{
    public interface IScriptLoader
    {
        /// <summary>
        /// Load the specified script, <paramref name="script"/>, and return a <see cref="IScript"/> object
        /// </summary>
        IScript Load(FileInfo script);

        /// <summary>
        /// Attempt to load the script <paramref name="script"/>, but if we fail to parse it, or if some other
        /// failure occurs, then just return null instead of throwing an exception to the caller.
        /// </summary>
        IScript TryLoad(FileInfo script);

        /// <summary>
        /// Load the specified content as a script (this is provided for unit testing)
        /// </summary>
        /// <param name="content">JavaScript or TypeScript content</param>
        IScript Load(string content);
    }
}

