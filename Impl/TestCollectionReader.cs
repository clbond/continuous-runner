using System.ComponentModel.Composition;

using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Impl
{
    using Frameworks.Jasmine;

    public class TestCollectionReader : ITestCollectionReader
    {
        [Import] private IJasmineReflection _reflector;

        #region Implementation of ISuiteReader

        public ITestCollection DefineTests(IScript script)
        {
            using (var engine = new V8ScriptEngine())
            {
                var collection = _reflector.Reflect(script, engine);
                
                engine.Execute(script.Content);

                return collection;
            }
        }

        #endregion
    }
}
