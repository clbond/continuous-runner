namespace TestRunner
{
    public interface IModuleReader
    {
        ModuleDefinition Get(IScript script);
    }
}
