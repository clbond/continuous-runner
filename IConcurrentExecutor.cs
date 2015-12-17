using System;

namespace ContinuousRunner
{
    /// <summary>
    /// Represents a queue of tests that need to be run
    /// </summary>
    public interface IConcurrentExecutor : IDisposable
    {
        /// <summary>
        /// Add an element to the queue of runnable items
        /// </summary>
        void Push(IPriorityWork work);        
    }
}
