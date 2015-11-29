using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Moq;

namespace Tests
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
    }
}
