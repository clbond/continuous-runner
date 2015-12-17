using System;
using System.Collections.Generic;
using System.Threading;

using NLog;

namespace ContinuousRunner.Impl
{
    using Work;

    public class ConcurrentExecutor : IConcurrentExecutor
    {
        private readonly ISet<IPriorityWork> _priorityQueue = new SortedSet<IPriorityWork>();

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly object _lock = new object();

        #region Implementation of IContinuousExecutor

        public void Push(IPriorityWork work)
        {
            lock (_lock)
            {
                _priorityQueue.Add(work);
            }

            _logger.Debug($"Pushed work onto queue: {work.Description}");

            ThreadPool.QueueUserWorkItem(
                state =>
                {
                    lock (_lock)
                    {
                        _priorityQueue.Remove(work);

                        _logger.Debug($"{_priorityQueue.Count} queued work items remain");

                    }
                    try
                    {
                        if (work.ExecuteAsync().Wait(TimeSpan.FromMinutes(1d)) == false)
                        {
                            _logger.Error($"Work execution timed out: {work.Description}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Error while executing work: {work.Description}: {ex.Message}");
                    }
                });
        }
        
        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {}

        #endregion
    }
}
