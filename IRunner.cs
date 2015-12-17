using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContinuousRunner
{
    using Data;

    public interface IRunner<in T>
    {
        IEnumerable<Task<TestResult>> RunAsync(T program);

        IEnumerable<Task<TestResult>> RunAsync(T program, TestSuite suite);

        Task<TestResult> RunAsync(T program, ITest test);
    }
}