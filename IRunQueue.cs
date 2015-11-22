using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestRunner
{
    public interface IRunQueue
    {
        void Push(IScript script);

        Task<IEnumerable<TestResult>> Run();
    }
}
