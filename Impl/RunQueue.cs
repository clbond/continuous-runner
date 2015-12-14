using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;

using Magnum.Threading;

using NLog;

namespace ContinuousRunner.Impl
{
    using Data;
    using System.Linq;

    public class RunQueue : IRunQueue
    {
        [Import]
        private readonly IScriptRunner _runner;

        private readonly ReaderWriterLockedObject<HashSet<IScript>> _waiting =
            new ReaderWriterLockedObject<HashSet<IScript>>(new HashSet<IScript>(), LockRecursionPolicy.SupportsRecursion);

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private volatile bool _running;

        #region Implementation of ITestQueue

        public void Push(IScript script)
        {
            var existing = false;

            _waiting.ReadLock(set => existing = set.Contains(script));

            if (existing)
            {
                return;
            }

            _waiting.WriteLock(
                set =>
                {
                    set.Add(script);

                    if (_running == false)
                    {
                        ThreadPool.QueueUserWorkItem(state => Run());
                    }
                });
        }

        public IEnumerable<Task<TestResult>> Run()
        {
            IScript[] scripts = null;

            _running = true;

            var tasks = new List<Task<TestResult>>();

            try
            {
                while (true)
                {
                    _waiting.ReadLock(set => scripts = set.ToArray());

                    if (scripts.Length == 0)
                    {
                        break;
                    }

                    foreach (var script in scripts)
                    {
                        var scriptTasks = _runner.RunAsync(script).ToList();

                        _logger.Info($"Running {script.TestCount} tests from {script.Description}");

                        tasks.AddRange(scriptTasks);

                        _waiting.WriteLock(set => set.Remove(script));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Uncaught exception while running tests: {ex.ToString()}");
            }
            finally
            {
                _running = false;
            }

            return tasks;
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
