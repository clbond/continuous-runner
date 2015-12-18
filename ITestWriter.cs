using System.Collections.Generic;

namespace ContinuousRunner
{
    public interface ITestWriter
    {
        void Write(IEnumerable<TestSuite> testSuites);
    }
}
