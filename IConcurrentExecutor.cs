using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContinuousRunner
{
    using Data;

    /// <summary>
    /// Represents a queue of tests that need to be run
    /// </summary>
    public interface IConcurrentExecutor : IDisposable
    {
        /// <summary>
        /// Add an element to the queue of runnable items
        /// </summary>
        void Push(IPriorityWork work);

        /// <summary>
        /// Return a collection representing concurrently executing tasks
        /// </summary>
        IEnumerable<IPriorityWork> GetExecutingWork();

        /// <summary>
        /// Get a collection representing the tasks waiting to be run (in the queue)
        /// </summary>
        IEnumerable<IPriorityWork> GetWaitingWork();

        /// <summary>
        /// Run all tests and return an individual <see cref="Task" /> for each one
        /// </summary>
        IEnumerable<Task<TestResult>> RunAsync();

        /// <summary>
        /// Run all tests and return a single <see cref="Task"/> representing all tests
        /// </summary>
        Task<TestResult[]> RunAllAsync();
    }
}
