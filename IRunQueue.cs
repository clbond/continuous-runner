using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContinuousRunner
{
    using Data;

    /// <summary>
    /// Represents a queue of tests that need to be run
    /// </summary>
    public interface IRunQueue : IDisposable
    {
        void Push(IScript script);

        /// <summary>
        /// Run all tests and return an individual <see cref="Task" /> for each one
        /// </summary>
        IEnumerable<Task<TestResult>> RunAsync();

        /// <summary>
        /// Run all tests and return a single <see cref="Task"/> representing all tests
        /// </summary>
        Task<IEnumerable<TestResult>> RunAllAsync();
    }
}
