using System;
using System.ComponentModel.Composition;

namespace ContinuousRunner.Impl
{
    public class DeterministicIdentifier : IDeterministicIdentifier
    {
        [Import] private readonly IHasher _hasher;

        #region Implementation of IDeterministicIdentifier

        public Guid GetIdentifier(params string[] identifiers)
        {
            var joined = string.Join(" > ", identifiers);

            return _hasher.GetHash(joined);
        }

        #endregion
    }
}
