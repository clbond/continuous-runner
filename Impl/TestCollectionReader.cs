using System.ComponentModel.Composition;

using Autofac;

using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Impl
{
    public class TestCollectionReader : ITestCollectionReader
    {
        [Import] private IComponentContext _componentContext;

        #region Implementation of ISuiteReader

        public ITestCollection DefineTests(IScript script)
        {
            using (var engine = new V8ScriptEngine())
            {
                var definer = _componentContext.Resolve<ITestCollection>(new TypedParameter(typeof (IScript), script));

                engine.AddHostObject(nameof(definer), definer);

                engine.Execute(
                    @"function describe(description, callback) {
                        definer.AddSuite(description, callback.toString());
                        callback();
                      }");

                engine.Execute(
                    @"function it(description, callback) {
                        definer.AddTest(description, callback.toString());
                      }");

                engine.Execute(script.Content);

                return definer;
            }
        }

        #endregion
    }
}
