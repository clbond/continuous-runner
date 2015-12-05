using System.Collections.Generic;

using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Frameworks.Jasmine
{
    public class JasmineImpl : IMockLibrary
    {
        private readonly Dictionary<string, object> _defines = new Dictionary<string, object>();
        
        #region Implementation of IMockLibrary

        public void Install(V8ScriptEngine v8)
        {
            //throw new System.NotImplementedException();
        }

        #endregion
    }
}
