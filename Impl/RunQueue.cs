using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Magnum.Extensions;
using Magnum.Threading;

using NLog;

namespace ContinuousRunner.Impl
{
    using Data;

    public class RunQueue : IRunQueue
    {
        [Import]
        private readonly IScriptRunner _runner;

        private readonly ReaderWriterLockedObject<HashSet<IScript>> _waiting =
            new ReaderWriterLockedObject<HashSet<IScript>>(new HashSet<IScript>(), LockRecursionPolicy.SupportsRecursion);

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        #region Implementation of ITestQueue

        public void Push(IScript script)
        {
            var existing = false;

            _waiting.ReadLock(set => existing = set.Contains(script));

            if (existing)
            {
                return;
            }

            _waiting.WriteLock(set => set.Add(script));
        }

        public IEnumerable<Task<TestResult>> RunAsync()
        {
            IScript[] scripts = null;

            var tasks = new List<Task<TestResult>>();

            // ReSharper disable once CatchAllClause
            try
            {
                _waiting.ReadLock(set => scripts = set.ToArray());

                if (scripts.Length == 0)
                {
                    return Enumerable.Empty<Task<TestResult>>();
                }

                foreach (var script in scripts)
                {
                    var scriptTasks = _runner.RunAsync(script).ToList();

                    _logger.Info($"Running {script.TestCount} tests from {script.Description}");

                    tasks.AddRange(scriptTasks);
                }

                _waiting.WriteLock(set => scripts.Each(s => set.Remove(s)));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Uncaught exception while running tests: {ex.ToString()}");

                throw new TestException($"Failed to run tests: {ex.Message}", ex);
            }

            return tasks;
        }

        public Task<TestResult[]> RunAllAsync()
        {
            var tasks = RunAsync();

            return Task.WhenAll(tasks.ToArray());
        }

        #endregion
        
        #region Implementation of IDisposable
        
        public void Dispose()
        {
            _waiting.Dispose();
        }
        
        #endregion

        #region Private methods

        #endregion
    }
}
