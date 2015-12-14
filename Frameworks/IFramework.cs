using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Frameworks
{
    public interface IFramework
    {
        Framework Framework { get; }

        void Install(IScript script, V8ScriptEngine v8);
    }
}
