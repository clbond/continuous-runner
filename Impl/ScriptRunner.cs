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
                var requireImpl = new Frameworks.RequireJs.FrameworkImpl(_instanceContext);
                requireImpl.Install(engine);

                var jasmineImpl = new Frameworks.Jasmine.FrameworkImpl();
                jasmineImpl.Install(engine);

                logger.Info($"Executing script: {test.Name}");

                var r = engine.Evaluate(script.Module.ModuleName, false, test.RawCode);

                return Task.FromResult(new TestResult {Status = TestStatus.Failed});
            }
        }

        #endregion
    }
}