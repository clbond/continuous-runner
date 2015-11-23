using System;
using System.Collections.Generic;
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
        private readonly ReaderWriterLockedObject<HashSet<IScript>> _waiting =
            new ReaderWriterLockedObject<HashSet<IScript>>(new HashSet<IScript>(), LockRecursionPolicy.SupportsRecursion);

        private Timer _timer;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region Implementation of ITestQueue

        public void Push(IScript script)
        {
            _waiting.WriteLock(
                set =>
                {
                    set.Add(script);

                    SetTimer();

                    if (set.Count >= Constants.MaximumQueueSize)
                    {
                        Task.Run(() => Run());
                    }
                });
        }

        public IEnumerable<Task<TestResult>> Run()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            IScript[] scripts = null;

            _waiting.ReadLock(set => scripts = set.ToArray());

            if (scripts.Length > 0)
            {
                foreach (var script in scripts)
                {
                    var scriptTasks = script.Suites.SelectMany(s => s.Tests).Select(t => t.Run()).ToList();
                    if (scriptTasks.Any())
                    {
                        _logger.Info($"Running {scriptTasks.Count} tests from {script.File.Name}");

                        Task.WhenAll(scriptTasks).ContinueWith(t => _waiting.WriteLock(set => set.Remove(script)));

                        foreach (var task in scriptTasks)
                        {
                            yield return task;
                        }
                    }
                }
            }
        }

        #endregion
        
        #region Implementation of IDisposable
        
        public void Dispose()
        {
            _timer?.Dispose();

            _waiting.Dispose();
        }
        
        #endregion

        #region Private methods

        private void SetTimer()
        {
            if (_timer == null)
            {
                _timer = new Timer(@obj => Task.Run(() => Run()));
                _timer.Change(Constants.QueueWait, TimeSpan.FromMilliseconds(-1));
            }
        }

        #endregion
    }
}
