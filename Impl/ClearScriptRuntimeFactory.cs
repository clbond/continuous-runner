using System;

using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Impl
{
    public class ClearScriptRuntimeFactory : IRuntimeFactory<ScriptEngine>
    {
        #region Implementation of IRuntimeFactory<out ScriptEngine>

        public ScriptEngine GetRuntime()
        {
            var engine = new V8ScriptEngine();

            InstallNativeTypes(engine);

            return engine;
        }

        #endregion

        #region Private methods

        private static void InstallNativeTypes(ScriptEngine engine)
        {
            engine.AddHostType("Action", typeof(Action<>));

            engine.AddHostType("Func", typeof (Func<>));

            engine.AddHostObject("mscorlib", new HostTypeCollection("mscorlib", "System.Core"));
        }

        #endregion
    }
}
