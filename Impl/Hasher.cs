using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ContinuousRunner.Impl
{
    public class Hasher : IHasher
    {
        #region Implementation of IHasher

        public Guid GetHash(string content)
        {
            using (var md5 = MD5.Create())
            {
                var buffer = Encoding.Default.GetBytes(content);

                var bytes = md5.ComputeHash(buffer);

                return new Guid(bytes);
            }
        }

        public Guid GetHash(FileInfo fileInfo)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = fileInfo.OpenRead())
                {
                    var bytes = md5.ComputeHash(stream);

                    return new Guid(bytes);
                }
            }
        }

        #endregion
    }
}