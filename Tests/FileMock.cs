using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Moq;

namespace ContinuousRunner.Tests
{
    public static class FileMock
    {
        private static readonly TempFileCollection _collection = new TempFileCollection();

        private static readonly Random _random = new Random();

        public static FileInfo FromString(string content)
        {
            var path = _collection.AddExtension(Convert.ToInt32(_random.Next()).ToString());

            var fs = new FileInfo(path);

            using (var stream = fs.OpenWrite())
            {
                var bytes = Encoding.UTF8.GetBytes(content);

                stream.Write(bytes, 0, bytes.Length);
            }

            return fs;
        }

        /// <summary>
        /// Get a reference to a test file (T FileInfo) or a test directory (T DirectoryInfo).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="components"></param>
        /// <returns></returns>
        public static T TestFile<T>(params string[] components) where T : FileSystemInfo
        {
            var path = new List<string>
                       {
                           Assembly.GetExecutingAssembly().Location,
                           @"..",
                           @".."
                       };

            path.AddRange(components);

            return (T) Activator.CreateInstance(typeof(T), Path.Combine(path.ToArray()));
        }
    }
}
