using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ContinuousRunner.Impl
{
    public class ScriptCollection : IScriptCollection
    {
        private readonly ISet<IScript> _scripts = new HashSet<IScript>(new ScriptComparer());

        #region Implementation of IScriptCollection

        public IEnumerable<IScript> GetChangedScripts()
        {
            return Enumerable.Empty<IScript>();
        }

        public IEnumerable<IScript> GetScripts()
        {
            return Enumerable.Empty<IScript>();
        }

        public void Add(IScript script)
        {
            _scripts.Add(script);
        }

        public void Remove(IScript script)
        {
            _scripts.Remove(script);
        }

        public IScript Find(Func<IScript, bool> matcher)
        {
            return _scripts.SingleOrDefault(matcher);
        }

        public IScript FindFile(FileInfo fileInfo)
        {
            return _scripts.FirstOrDefault(s => s.File?.FullName == fileInfo.FullName);
        }

        #endregion
    }
}
