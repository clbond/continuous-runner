using System;
using System.Collections.Generic;

namespace ContinuousRunner
{
    public interface IReferenceTree
    {
        /// <summary>
        /// Get a collection of scripts that depend directly or indirectly on <paramref name="origin"/>
        /// </summary>
        IEnumerable<IScript> GetDependents(IScript origin);

        /// <summary>
        /// Get a collection of scripts that <paramref name="origin"/> depends upon, directly or indirectly
        /// </summary>
        IEnumerable<IScript> GetDependencies(IEnumerable<IScript> scripts, IScript origin);
    }
}