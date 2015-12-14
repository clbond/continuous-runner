using System.IO;

namespace ContinuousRunner
{
    public interface ILoader<TSource> where TSource : IProjectSource
    {
        /// <summary>
        /// Load the specified script, <paramref name="script"/>, and return a <see cref="IScript"/> object
        /// </summary>
        TSource Load(FileInfo script);

        /// <summary>
        /// Attempt to load the script <paramref name="script"/>, but if we fail to parse it, or if some other
        /// failure occurs, then just return null instead of throwing an exception to the caller.
        /// </summary>
        TSource TryLoad(FileInfo script);

        /// <summary>
        /// Load the specified content as a script (this is provided for unit testing)
        /// </summary>
        /// <param name="content">JavaScript or TypeScript content</param>
        TSource Load(string content);

        /// <summary>
        /// Load a script based on a module reference string ('Namespace/Foo/Bar'). The reference,
        /// <paramref name="relativeReference"/>, can be relative to the script <paramref name="fromScript"/>.
        /// </summary>
        TSource LoadModule(TSource fromScript, string relativeReference);

        /// <summary>
        /// Load a script based on an absolute (not relative) module reference string
        /// </summary>
        TSource LoadModule(string absoluteReference);
    }
}

