using System;

namespace ContinuousRunner
{
    using Data;

    public interface IModuleReader
    {
        ModuleDefinition Get(IScript script, Func<string, IScript> referenceLoader);
    }
}
