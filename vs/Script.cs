using System;
using System.Collections.Generic;
using System.IO;

namespace TestRunner
{
    public class Script : IScript
    {
        private readonly FileInfo _fileInfo;

        public Script(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        #region IScript implementation

        public IEnumerable<TestSuite> GetSuites()
        {
            using (var stream = _fileInfo.OpenRead())
            {
                using (var sr = new StreamReader(stream))
                {
                    yield break;
                }
            }
        }

        public FileInfo File
        {
            get
            {
                return _fileInfo;
            }
        }

        #endregion
    }
}