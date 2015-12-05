using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContinuousRunner
{
    using Data;

    public interface IScriptRunner
    {
        IEnumerable<Task<TestResult>> Run(IScript script);

        IEnumerable<Task<TestResult>> Run(IScript script, TestSuite suite);

        Task<TestResult> Run(IScript script, ITest test);
    }
}