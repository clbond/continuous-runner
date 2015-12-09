using System.IO;

namespace ContinuousRunner
{
    using System.Collections.Generic;

    public interface IScriptLoader
    {
        IEnumerable<IScript> GetScripts();

        IEnumerable<IScript> GetTestScripts();

        IScript Load(FileInfo script);
        
        IScript Load(string content);
    }
}

