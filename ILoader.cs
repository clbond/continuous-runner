using System.IO;

namespace ContinuousRunner
{
    public interface ILoader<out TSource> where TSource : IProjectSource
    {
        /// <summary>
        /// Load the specified file, <paramref name="file"/>, and return a <see cref="TSource"/> object
        /// </summary>
        TSource Load(FileInfo file);

        /// <summary>
        /// Attempt to load the file <paramref name="file"/>, but if we fail to parse it, or if some other
        /// failure occurs, then just return null instead of throwing an exception to the caller.
        /// </summary>
        TSource TryLoad(FileInfo file);

        /// <summary>
        /// Load the specified content as a file (this is provided for unit testing)
        /// </summary>
        /// <param name="content">Javafile or Typefile content</param>
        TSource Load(string content);
        
        /// <summary>
        /// Load a file based on an absolute (not relative) module reference string
        /// </summary>
        TSource LoadModule(string absoluteReference, string fromModule);
    }
}

