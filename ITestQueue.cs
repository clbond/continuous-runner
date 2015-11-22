using System.Collections.Generic;

namespace TestRunner
{
    public interface ITestQueue
    {
        void Push(IScript script);

        IEnumerable<TestResult> Execute();
    }
}
