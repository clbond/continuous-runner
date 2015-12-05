using System.IO;

namespace ContinuousRunner
{
    using System.Collections.Generic;

    public interface IScriptLoader
    {
        IEnumerable<IScript> GetScripts();

        IEnumerable<IScript> GetTestScripts();

        IScript Load(FileInfo script);

        /// <summary>
        /// Load from contenet instead of from file (for testing primarily)
        /// </summary>
        IScript Load(string content);
    }
}

