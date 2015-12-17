using System;
using System.IO;

namespace ContinuousRunner.Tests.Mock
{
    public interface IMockFile : IDisposable
    {
        FileInfo FromString(string extension, string content);

        /// <summary>
        /// Get a reference to a test file (T FileInfo) or a test directory (T DirectoryInfo).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="components"></param>
        /// <returns></returns>
        T TestFile<T>(params string[] components) where T : FileSystemInfo;
    }
}
