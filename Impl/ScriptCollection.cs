using System;
using System.Collections.Generic;

namespace ContinuousRunner.Impl
{
    public class ScriptCollection : IScriptCollection
    {
        #region Implementation of IScriptCollection

        public IEnumerable<IScript> GetChangedScripts()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IScript> GetScripts()
        {
            throw new System.NotImplementedException();
        }

        public void Add(IScript script)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(IScript script)
        {
            throw new System.NotImplementedException();
        }

        public IScript Find(Func<IScript, bool> matcher)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
