using Microsoft.ClearScript;

namespace ContinuousRunner.Frameworks.Jasmine
{
    public class FrameworkImpl : IFramework
    {
        #region Implementation of IFramework

        public Framework Framework => Framework.Jasmine;

        public void Install(IProjectSource source, ScriptEngine engine)
        {
            // TODO(cbond): Find and run Jasmine framework code
        }

        #endregion
    }
}
