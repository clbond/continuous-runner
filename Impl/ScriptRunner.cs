using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using ContinuousRunner.Frameworks.Jasmine;
using ContinuousRunner.Frameworks.RequireJs;

using Microsoft.ClearScript.V8;

using NLog;

namespace ContinuousRunner.Impl
{
    using Data;

    public class ScriptRunner : IScriptRunner
    {
        [Import] public readonly IInstanceContext _instanceContext;
        
        #region Implementation of IScriptRunner

        public IEnumerable<Task<TestResult>> RunAsync(IScript script)
        {
            return script.Suites.SelectMany(s => s.Tests.Select(t => ExecuteTest(script, t)));
        }

        public IEnumerable<Task<TestResult>> RunAsync(IScript script, TestSuite suite)
        {
            return suite.Tests.Select(t => ExecuteTest(script, t));
        }

        public Task<TestResult> RunAsync(IScript script, ITest test)
        {
            return ExecuteTest(script, test);
        }

        #endregion

        #region Private methods

        private Task<TestResult> ExecuteTest(IScript script, ITest test)
        {
            var logger = LogManager.GetCurrentClassLogger();

            using (var engine = new V8ScriptEngine())
            {
                var requireImpl = new RequireImpl(_instanceContext);
                requireImpl.Install(engine);

                var jasmineImpl = new JasmineImpl();
                jasmineImpl.Install(engine);

                logger.Info($"Executing script: {test.Name}");

         //       var run = $"{test.RawCode}();";

                var run = @"var x = 0;";

                engine.Evaluate(run);

                return Task.FromResult(new TestResult {Status = TestStatus.Failed});
            }
        }

        #endregion
    }
}