using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Frameworks
{
    public interface IMockLibrary
    {
        void Install(V8ScriptEngine v8);
    }
}
