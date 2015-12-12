using System.Collections.Generic;
using System.ComponentModel.Composition;

using Autofac;

using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Extractors
{
    using Data;

    public class SuiteReader : ISuiteReader
    {
        [Import] private IComponentContext _componentContext;

        #region Implementation of ISuiteReader

        public Definer Define(IScript script)
        {
            using (var engine = new V8ScriptEngine())
            {
                var definer = _componentContext.Resolve<Definer>(new TypedParameter(typeof (IScript), script));

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
