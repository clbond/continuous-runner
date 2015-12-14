using System;

using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Frameworks.Jasmine
{
    public class FrameworkImpl : IFramework
    {
        #region Implementation of IFramework

        public Framework Framework => Framework.Jasmine;

        public void Install(V8ScriptEngine v8)
        {
            throw new NotImplementedException("Needed: Code to find and run Jasmine library against a V8 execution context");
        }

        #endregion
    }
}
