using System;
using System.IO;

namespace ContinuousRunner
{
    public interface IHasher
    {
        /// <summary>
        /// Generate an MD5 hash of the specified content, <paramref name="content"/>
        /// </summary>
        Guid GetHash(string content);

        /// <summary>
        /// Generate an MD5 hash of the specified file, <paramref name="fileInfo"/>
        /// </summary>
        Guid GetHash(FileInfo fileInfo);
    }
}
