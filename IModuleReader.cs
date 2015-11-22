namespace TestRunner
{
    public interface IModuleReader
    {
        ModuleDefinition ReadModule(IScript script);
    }
}
