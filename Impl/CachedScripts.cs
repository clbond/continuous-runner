using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Magnum.Extensions;
using NLog;

namespace ContinuousRunner.Impl
{
    public class CachedScripts : ICachedScripts
    {
        [Import] private readonly IHasher _hasher;

        [Import] private readonly ILoader<IScript> _loader;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger(); 

        private readonly IDictionary<string, CachedScriptItem> _cache = new Dictionary<string, CachedScriptItem>();

        #region Implementation of ICachedScripts

        private CachedScriptItem? GetCached(FileInfo fileInfo, out Guid hash)
        {
            hash = Guid.Empty;

            if (_cache.ContainsKey(fileInfo.FullName) == false)
            {
                return null;
            }

            var scriptItem = _cache[fileInfo.FullName];
            
            if (scriptItem.Updated >= fileInfo.LastWriteTime)
            {
                hash = scriptItem.Hash;

                return scriptItem;
            }

            // If the file has been modified since we loaded it, then compare the hashes
            hash = _hasher.GetHash(fileInfo);

            if (scriptItem.Hash == hash)
            {
                return scriptItem;
            }

            _logger.Debug($"Script {fileInfo.Name} is stale (hash: {scriptItem.Hash.ToString()} vs {hash.ToString()}); removing");

            return null;
        }

        public IScript Load(FileInfo fileInfo)
        {
            try
            {
                Guid hash;
                var existingItem = GetCached(fileInfo, out hash);
                if (existingItem.HasValue)
                {
                    var item = existingItem.Value;

                    item.Accessed = DateTime.Now;

                    return item.Script;
                }

                if (hash == Guid.Empty)
                {
                    hash = _hasher.GetHash(fileInfo);
                }

                var loaded = _loader.Load(fileInfo);

                var cachedItem = new CachedScriptItem
                                 {
                                     Accessed = DateTime.Now,
                                     Updated = DateTime.Now,
                                     Script = loaded,
                                     Hash = hash
                                 };

                _cache[fileInfo.FullName] = cachedItem;

                _logger.Debug($"Inserted script into cache: {fileInfo.Name}: hash {hash.ToString()}");

                return loaded;
            }
            finally
            {
                RemoveStale();
            }
        }

        private void RemoveStale()
        {
            if (_cache.Count < Constants.ScriptCacheSize)
            {
                return;
            }

            // Get the items that have not been used or referenced in a while and remove them from the cache.
            // The goal is to keep the cache at a maximum size, with the most recently-used scripts always
            // being retained, and the ones that have not been referenced in a while get removed.
            var matching =
                _cache.Select(kvp => Tuple.Create(kvp.Key, kvp.Value.Updated))
                      .OrderByDescending(t => t.Item2)
                      .Skip(Constants.ScriptCacheSize)
                      .Select(kvp => kvp.Item1)
                      .ToArray();

            if (matching.Length > 0)
            {
                _logger.Debug($"Removing {matching.Length} stale items from script cache");

                matching.Each(k => _cache.Remove(k));
            }
        }

        public void Remove(FileInfo fileInfo)
        {
            try
            {
                _cache.Remove(fileInfo.FullName);
            }
            finally
            {
                RemoveStale();
            }
        }

        public void Remove(IScript script)
        {
            if (script.File != null)
            {
                Remove(script.File);
            }
            else
            {
                var keys = _cache.Where(s => s.Value.Script == script).Select(kvp => kvp.Key).ToArray();

                keys.Each(k => Remove(new FileInfo(k)));
            }
        }

        #endregion
    }
}
