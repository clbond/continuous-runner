using System.Collections.Generic;

namespace ContinuousRunner
{
    using Data;

    public interface ITestWriter
    {
        void Write(IEnumerable<TestSuite> testSuites);
    }
}
