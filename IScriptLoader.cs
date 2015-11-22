using System.IO;

namespace TestRunner
{
    public interface IScriptLoader
    {
        IScript Load(FileInfo script);
    }
}

