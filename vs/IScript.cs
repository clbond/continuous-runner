using System.IO;
using System.Collections.Generic;

namespace Contestual
{
    public interface IScript
    {
        /// <summary>
        /// The file associated with this JavaScript item
        /// </summary>
        FileInfo File { get; }

        /// <summary>
        /// Get a collection of test suites defined in this script
        /// </summary>
        /// <returns>The suites.</returns>
        IEnumerable<TestSuite> GetSuites();
    }
}

