using System;
using System.Threading.Tasks;

namespace ContinuousRunner
{
    /// <summary>
    /// Represents a queue of tests that need to be run
    /// </summary>
    public interface IConcurrentExecutor : IDisposable
    {
        bool PendingWork { get; }

        /// <summary>
        /// Add an element to the queue of runnable items
        /// </summary>
        Task<IExecutionResult> ExecuteAsync(IPriorityWork work);        
    }
}
