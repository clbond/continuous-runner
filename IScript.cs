using System;
using System.IO;
using System.Collections.Generic;

namespace TestRunner
{
    public interface IScript : IComparable<IScript>
    {
        /// <summary>
        /// The file associated with this JavaScript item
        /// </summary>
        FileInfo File { get; }

        /// <summary>
        /// An abstract syntax tree of a parsed JavaScript file
        /// </summary>
        SyntaxTree SyntaxTree { get; }

        /// <summary>
        /// Get a collection of test suites defined in this script
        /// </summary>
        /// <returns>The suites.</returns>
        IEnumerable<TestSuite> GetSuites();
    }
}