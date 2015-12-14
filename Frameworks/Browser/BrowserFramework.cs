using Microsoft.ClearScript;

namespace ContinuousRunner.Frameworks.Browser
{
    public class BrowserFramework : IFramework
    {
        #region Implementation of IFramework

        public Framework Framework => Framework.None;

        public void Install(IProjectSource source, ScriptEngine engine)
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
