using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Frameworks.Jasmine
{
    public class FrameworkImpl : IFramework
    {
        #region Implementation of IFramework

        public Framework Framework => Framework.Jasmine;

        public void Install(IProjectSource source, V8ScriptEngine engine)
        {
            // TODO(cbond): Find and run Jasmine framework code
        }

        #endregion
    }
}
