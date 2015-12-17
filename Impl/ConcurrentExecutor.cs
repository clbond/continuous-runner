using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using NLog;

namespace ContinuousRunner.Impl
{
    using Data;

    public class ConcurrentExecutor : IConcurrentExecutor
    {
        private readonly ISet<IPriorityWork> _priorityQueue = new SortedSet<IPriorityWork>();

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        #region Implementation of IContinuousExecutor

        public void Push(IPriorityWork work)
        {
            _priorityQueue.Add(work);
        }

        public IEnumerable<IPriorityWork> GetExecutingWork()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPriorityWork> GetWaitingWork()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Task<TestResult>> RunAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TestResult[]> RunAllAsync()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
