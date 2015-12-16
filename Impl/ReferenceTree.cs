using System.Collections.Generic;
using System.Linq;

namespace ContinuousRunner.Impl
{
    public class ReferenceTree : IReferenceTree
    {
        #region Implementation of IReferenceTree
        
        public IEnumerable<IScript> GetDependents(IScript origin)
        {
            var refs = origin.Module?.References;
            if (refs == null)
            {
                yield break;
            }

            foreach (var @ref in refs)
            {
                yield return @ref;

                foreach (var innerRef in GetDependents(@ref))
                {
                    yield return innerRef;
                }
            }
        }

        public IEnumerable<IScript> GetDependencies(IEnumerable<IScript> scripts, IScript origin)
        {
            return scripts.SelectMany(GetDependents).Where(s => ContainsReference(s, origin));
        }

        #endregion

        #region Private methods

        private bool ContainsReference(IScript @ref, IScript origin)
        {
            return GetDependents(@ref).Any(d => d == origin);
        }

        #endregion
    }
}
