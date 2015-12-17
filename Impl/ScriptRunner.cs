using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

using NLog;

namespace ContinuousRunner.Impl
{
    using Data;
    using Frameworks;

    public class ScriptRunner : IRunner<IScript>
    {
        [Import] private readonly IFrameworkDetector _frameworkDetector;

        [Import] private readonly IPublisher _publisher;

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

        private Task<TestResult> RunTestAsync(IProjectSource script, ITest test)
        {
            var completionSource = new TaskCompletionSource<TestResult>();

            ThreadPool.QueueUserWorkItem(state => RunTest(completionSource, script, test));

            return completionSource.Task;
        }

        private void RunTest(TaskCompletionSource<TestResult> completionSource, IProjectSource source, ITest test)
        {
            var logger = LogManager.GetCurrentClassLogger();

            var engine = GetEngine();
            try
            {
                logger.Debug("Installing frameworks into V8 execution context");

                _frameworkDetector.InstallFrameworks(source, source.Frameworks, engine);

                logger.Info($"Executing script: {test.Name}");

                var code = $"{{{WrapInCallExpression(test.RawCode)}}}";

                Func<Status, TestResult> transform = status => new TestResult
                                                                   {

                                                                       Test = test,
                                                                       Status = status,
                                                                       Logs = source.Logs
                                                                   };

                TestResult testResult;
                try
                {
                    engine.Evaluate(code);

                    testResult = transform(Status.Passed);
                }
                catch (Exception ex)
                {
                    source.Logs.Add(Tuple.Create(DateTime.Now, Severity.Error, $"Uncaught exception: {ex}"));

                    testResult = transform(Status.Failed);
                }

                test.Result = testResult;

                completionSource.SetResult(testResult);

                _publisher.Publish(testResult);
            }
            catch (Exception ex)
            {
                completionSource.SetException(ex);
            }
            finally
            {
                engine.Dispose();
            }
        }

        private static string WrapInCallExpression(string code)
        {
            return $"({code})();";
        }

        private static ScriptEngine GetEngine()
        {
            return new V8ScriptEngine
                   {
                       AllowReflection = true,
                       DisableTypeRestriction = false,
                       EnableAutoHostVariables = true,
                       EnableNullResultWrapping = true,
                       FormatCode = false,
                       UseReflectionBindFallback = true
                   };
        }
    }

    #endregion
}