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

        private readonly Logger _logger = LogManager.GetCurrentClassLogger(); 

        private readonly IDictionary<string, CachedScriptItem> _cache = new Dictionary<string, CachedScriptItem>();

        #region Implementation of ICachedScripts

        public IScript Get(FileInfo fileInfo, Func<FileInfo, IScript> load)
        {
            if (load == null)
            {
                throw new ArgumentNullException(nameof(load));
            }

            try
            {
                var hash = _hasher.GetHash(fileInfo);

                if (_cache.ContainsKey(fileInfo.FullName))
                {
                    var scriptItem = _cache[fileInfo.FullName];
                    if (scriptItem.Hash == hash)
                    {
                        scriptItem.Accessed = DateTime.Now;

                        return scriptItem.Script;
                    }
                    else
                    {
                        _logger.Debug(
                            $"Script {fileInfo.Name} is stale (hash: {scriptItem.Hash} vs {hash}); removing");
                    }
                }

                var loaded = load(fileInfo);

                var cachedItem = new CachedScriptItem
                                 {
                                     Script = loaded,
                                     Accessed = DateTime.Now,
                                     Hash = hash
                                 };

                _cache[fileInfo.FullName] = cachedItem;

                _logger.Debug($"Inserted script into cache: {fileInfo.Name}: hash {hash}");

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
                _cache.Select(kvp => Tuple.Create(kvp.Key, kvp.Value.Accessed))
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
