using System.Collections.Generic;

using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Extractors
{
    using Data;

    public class SuiteReader : ISuiteReader
    {
        #region Implementation of IScriptRunner

        public IEnumerable<TestSuite> GetTests(IScript script)
        {
            using (var engine = new V8ScriptEngine())
            {
                const string addSuite =
                    @"function describe(description, callback) {
                        testCollection.AddSuite(description, callback.toString());
                        callback();
                      }";

                const string addTest =
                    @"function it(description, callback) {
                        testCollection.AddTest(description, callback.toString());
                      }";

                var collection = new TestCollection {Script = script};

                engine.AddHostObject("testCollection", collection);
                engine.Execute(addSuite);
                engine.Execute(addTest);

                engine.Execute(script.Content);

                return collection.Suites;
            }
        }

        #endregion
    }
}
