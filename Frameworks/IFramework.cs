using Microsoft.ClearScript;

namespace ContinuousRunner.Frameworks
{
    public interface IFramework
    {
        Framework Framework { get; }

        void Install(IProjectSource source, ScriptEngine engine);
    }
}
