using System;
using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Frameworks.Browser
{
    public class BrowserFramework : IFramework
    {
        #region Implementation of IFramework

        public Framework Framework => Framework.None;

        public void Install(IProjectSource source, V8ScriptEngine engine)
        {
            if (source is IScript == false)
            {
                return;
            }

            var console = new BrowserConsole((IScript) source);

            engine.AddHostObject(nameof(console), console);
        }

        #endregion
    }
}
