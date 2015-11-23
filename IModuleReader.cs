namespace ContinuousRunner
{
    using Data;

    public interface IModuleReader
    {
        ModuleDefinition Get(IScript script);
    }
}
