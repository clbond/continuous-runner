using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContinuousRunner
{
    using Data;

    public interface IScriptRunner
    {
        IEnumerable<Task<TestResult>> RunAsync(IScript script);

        IEnumerable<Task<TestResult>> RunAsync(IScript script, TestSuite suite);

        Task<TestResult> RunAsync(IScript script, ITest test);
    }
}