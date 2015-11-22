using System.Collections.Generic;

namespace TestRunner
{
    public interface IScriptFinder
    {
        IEnumerable<IScript> GetScripts();
    }
}
