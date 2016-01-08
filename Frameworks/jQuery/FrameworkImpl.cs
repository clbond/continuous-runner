using System.ComponentModel.Composition;
using Microsoft.ClearScript;

namespace ContinuousRunner.Frameworks.jQuery
{
    public class FrameworkImpl : IFramework
    {
        [Import] private readonly IInstanceContext _instanceContext;

        #region Implementation of IFramework

        public Framework Framework => Framework.jQuery;

        public void Install(IProjectSource source, ScriptEngine engine)
        {
            if (source is IScript == false)
            {
                return;
            }


        }

        #endregion
    }
}
