using System;
using System.Collections.Generic;
using System.IO;

namespace ContinuousRunner
{
    public interface IScriptCollection
    {
        /// <summary>
        /// Get a collection of all known scripts
        /// </summary>
        IEnumerable<IScript> GetScripts();

        /// <summary>
        /// Get a collection of all known test scripts
        /// </summary>
        IEnumerable<IScript> GetTestScripts();

        /// <summary>
        /// Get the complete set of product scripts, minus the collection returned by <see cref="GetTestScripts"/>
        /// </summary>
        IEnumerable<IScript> GetProductScripts();

        /// <summary>
        /// Add a new script to the collection
        /// </summary>
        /// <param name="script">A product script or a test script</param>
        void Add(IScript script);

        /// <summary>
        /// Remove the specified script from this collection
        /// </summary>
        void Remove(IScript script);

        /// <summary>
        /// Find a matching script from this collection
        /// </summary>
        IScript FindScript(Func<IScript, bool> matcher);

        /// <summary>
        /// Find a matching test script from this collection
        /// </summary>
        IScript FindTestScript(Func<IScript, bool> matcher);

        /// <summary>
        /// Find a <see cref="IScript"/> reference using a file, <paramref name="fileInfo"/>
        /// </summary>
        IScript GetScriptFromFile(FileInfo fileInfo);
    }
}
