using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.ClearScript.V8;

using NLog;

namespace ContinuousRunner.Impl
{
    using Data;
    using Frameworks;

    public class ScriptRunner : IScriptRunner
    {
        [Import] private readonly IFrameworkDetector _frameworkDetector;
        
        #region Implementation of IScriptRunner

        public IEnumerable<Task<TestResult>> RunAsync(IScript script)
        {
            return script.Suites.SelectMany(s => s.Tests.Select(t => RunTestAsync(script, t)));
        }

        public IEnumerable<Task<TestResult>> RunAsync(IScript script, TestSuite suite)
        {
            return suite.Tests.Select(t => RunTestAsync(script, t));
        }

        public Task<TestResult> RunAsync(IScript script, ITest test)
        {
            return RunTestAsync(script, test);
        }

        #endregion

        #region Private methods

        private Task<TestResult> RunTestAsync(IScript script, ITest test)
        {
            var logger = LogManager.GetCurrentClassLogger();

            var completionSource = new TaskCompletionSource<TestResult>();

            Task.Run(
                () =>
                {
                    var engine = new V8ScriptEngine();
                    try
                    {
                        logger.Debug("Installing frameworks into V8 execution context");

                        _frameworkDetector.InstallFrameworks(script, script.Frameworks, engine);

                        logger.Info($"Executing script: {test.Name}");

                        var r = engine.Evaluate(script.Module.ModuleName, false, test.RawCode);
                    }
                    finally
                    {
                        engine.Dispose();
                    }

                    completionSource.SetResult(new TestResult {Status = TestStatus.Failed});
                });

            return completionSource.Task;
        }
    }

    #endregion
}