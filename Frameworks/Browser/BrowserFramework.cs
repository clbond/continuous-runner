using System;
using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Frameworks.Browser
{
    public class BrowserFramework : IFramework
    {
        #region Implementation of IFramework

        public Framework Framework => Framework.None;

        public void Install(IScript script, V8ScriptEngine engine)
        {
            var console = new BrowserConsole(script);

            engine.AddHostObject(nameof(console), console);
        }

        #endregion
    }
}
