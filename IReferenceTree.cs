using System;
using System.Collections.Generic;

namespace ContinuousRunner
{
    public interface IReferenceTree : IDisposable
    {
        /// <summary>
        /// Take a module path and resolve it into an <see cref="IScript"/> object
        /// </summary>
        IScript GetScriptFromModuleReference(string module);

        /// <summary>
        /// Get a collection of scripts that depend directly or indirectly on <paramref name="script"/>
        /// </summary>
        IEnumerable<IScript> GetDependents(IScript script);

        /// <summary>
        /// Get a collection of scripts that <paramref name="script"/> depends upon, directly or indirectly
        /// </summary>
        IEnumerable<IScript> GetDependencies(IScript script);
    }
}