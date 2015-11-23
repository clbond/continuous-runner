using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContinuousRunner
{
    using Data;

    /// <summary>
    /// Represents a queue of tests that need to be run
    /// </summary>
    public interface IRunQueue
    {
        void Push(IScript script);

        IEnumerable<Task<TestResult>> Run();
    }
}
