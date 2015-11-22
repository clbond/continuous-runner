using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Contestual
{
    public class ScriptLoader : IScriptLoader
    {
        #region Implementation of IScriptLoader

        public IEnumerable<IScript> Load(DirectoryInfo root)
        {
            return root.GetFiles("*.js", SearchOption.AllDirectories).Select(f => new Script(f));
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {

        }

        #endregion
    }
}

