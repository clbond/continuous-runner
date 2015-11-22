using System;
using System.Collections.Generic;
using System.IO;

namespace TestRunner
{
    public class Script : IScript
    {
        #region Implementation of IScript

        public FileInfo File { get; set; }

        public SyntaxTree SyntaxTree { get; set; }

        public IEnumerable<TestSuite> GetSuites()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IComparable<in IScript>

        public int CompareTo(IScript other)
        {
            return string.Compare(File.Name, other.File.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}