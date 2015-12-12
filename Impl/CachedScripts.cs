using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

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

        public void Remove(FileInfo fileInfo)
        {
            _cachedScripts.Remove(fileInfo);
        }

        public void Remove(IScript script)
        {
            if (script.File != null)
            {
                Remove(script.File);
            }
            else
            {
                var keys = _cachedScripts.Where(s => s.Value.Item2 == script).Select(kvp => kvp.Key).ToArray();

                foreach (var key in keys)
                {
                    Remove(key);
                }
            }
        }

        #endregion
    }
}
