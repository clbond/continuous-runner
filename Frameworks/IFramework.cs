using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Frameworks
{
    public interface IFramework
    {
        Framework Framework { get; }

        void Install(IProjectSource source, V8ScriptEngine engine);
    }
}
