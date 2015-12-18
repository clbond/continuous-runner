using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Magnum.Threading;

using NLog;

namespace ContinuousRunner.Impl
{
    public class ConcurrentExecutor : IConcurrentExecutor
    {
        private readonly ILockedObject<IList<Task<IExecutionResult>>> _active =
            new ReaderWriterLockedObject<IList<Task<IExecutionResult>>>(new List<Task<IExecutionResult>>());

        #region Implementation of IContinuousExecutor

        public bool PendingWork
        {
            get
            {
                var pending = false;
                _active.ReadLock(active => pending = active.Count > 0);

                return pending;
            }
        }

        public Task<IExecutionResult> ExecuteAsync(IPriorityWork work)
        {
            var logger = LogManager.GetCurrentClassLogger();

            logger.Debug($"Pushed work onto queue: {work.Description}");

            var completionSource = new TaskCompletionSource<IExecutionResult>();

            _active.WriteLock(active => active.Add(completionSource.Task));

            Action executor =
                () =>
                {
                    try
                    {
                        _active.WriteLock(active => active.Remove(completionSource.Task));

                        var task = work.ExecuteAsync();

                        if (task.Wait(TimeSpan.FromSeconds(10d)) == false)
                        {
                            logger.Error($"Work execution timed out: {work.Description}");

                            completionSource.SetException(
                                new TestException(
                                    $"Timed out waiting for script execution to complete: {work.Description}"));
                        }
                        else
                        {
                            completionSource.SetResult(task.Result);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, $"Error while executing work: {work.Description}: {ex.Message}");

                        completionSource.SetException(
                            new TestException(
                                $"Uncaught exception while executing job: {work.Description}: {ex.Message}",
                                ex));
                    }
                };

            Task.Run(executor);

            return completionSource.Task;
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _active.WriteLock(
                active =>
                {
                    if (active.Any() == false)
                    {
                        return;
                    }

                    var task = Task.WhenAll(active);

                    task.Wait(TimeSpan.FromSeconds(1d));
                });

            _active.Dispose();
        }

        #endregion
    }
}