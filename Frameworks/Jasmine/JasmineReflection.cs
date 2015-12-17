using System.ComponentModel.Composition;
using Autofac;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Frameworks.Jasmine
{
    public class JasmineReflection : IJasmineReflection
    {
        [Import] private readonly IComponentContext _componentContext;

        public ITestCollection Reflect(IScript script, ScriptEngine engine)
        {
            var testCollection = _componentContext.Resolve<ITestCollection>(new TypedParameter(typeof(IScript), script));

            engine.AddHostObject(nameof(testCollection), testCollection);

            engine.Execute(
                @"function describe(description, callback) {
                        testCollection.AddSuite(description, callback.toString());
                        callback();
                      }");

            engine.Execute(
                @"function it(description, callback) {
                        testCollection.AddTest(description, callback.toString());
                      }");
            
            return testCollection;
        }
    }
}
