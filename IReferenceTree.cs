using System;
using System.Collections.Generic;

namespace ContinuousRunner
{
    public interface IReferenceTree : IDisposable
    {
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