using System;
using System.IO;

namespace ContinuousRunner
{
    public interface IModuleReader
    {
        ModuleDefinition Get(IScript script, Func<string, IScript> referenceLoader);

        string GetModuleNameFromScript(FileInfo fileInfo);
    }
}
