using System;
using System.Collections.Generic;
using System.IO;

namespace Contestual
{
    public interface IScriptLoader : IDisposable
    {
        /// <summary>
        /// Load all of the scripts in the specified root, <paramref name="root"/>
        /// </summary>
        IEnumerable<IScript> Load(DirectoryInfo root);
    }
}

