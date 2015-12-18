using System;
using System.Collections.Generic;
using System.IO;

namespace ContinuousRunner
{
    using Frameworks;

    public interface IProjectSource : IComparable<IProjectSource>
    {
        /// <summary>
        /// The file associated with this JavaScript item
        /// </summary>
        FileInfo File { get; }

        /// <summary>
        /// Get a collection of test suites defined in this script
        /// </summary>
        IEnumerable<TestSuite> Suites { get; }

        /// <summary>
        /// Raw JavaScript content of this script
        /// </summary>
        string Content { get; }

        /// <summary>
        /// A simple description of this script for display purposes
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The number of tests defined in this script (may change on reload)
        /// </summary>
        int TestCount { get; }

        /// <summary>
        /// What frameworks does this script use? (eg., RequireJS)
        /// </summary>
        Framework Frameworks { get; }

        /// <summary>
        /// Logs produced when running this script
        /// </summary>
        IList<Tuple<DateTime, Severity, string>> Logs { get; }

        /// <summary>
        /// Reload the contents of this script since it has changed on disk
        /// </summary>
        void Reload();
    }
}