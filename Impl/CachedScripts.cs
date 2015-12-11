using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace ContinuousRunner.Impl
{
    public class CachedScripts : ICachedScripts
    {
        [Import] private readonly IHasher _hasher;

        [Import] private readonly IScriptLoader _loader;

        private readonly IDictionary<FileInfo, Tuple<Guid, IScript>> _cachedScripts =
            new Dictionary<FileInfo, Tuple<Guid, IScript>>();

        #region Implementation of ICachedScripts

        public IScript Get(FileInfo fileInfo)
        {
            var newHash = _hasher.GetHash(fileInfo);

            if (_cachedScripts.ContainsKey(fileInfo))
            {
                var tuple = _cachedScripts[fileInfo];

                if (newHash == tuple.Item1)
                {
                    return tuple.Item2;
                }
            }

            var loaded = _loader.Load(fileInfo);

            _cachedScripts[fileInfo] = Tuple.Create(newHash, loaded);

            return loaded;
        }

        #endregion
    }
}
