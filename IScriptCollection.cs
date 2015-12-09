using System;
using System.Collections.Generic;

namespace ContinuousRunner
{
    public interface IScriptCollection
    {
        IEnumerable<IScript> GetChangedScripts();

        IEnumerable<IScript> GetScripts();

        void Add(IScript script);

        void Remove(IScript script);

        IScript Find(Func<IScript, bool> matcher);
    }
}
