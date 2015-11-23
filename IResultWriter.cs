using System.Collections.Generic;

namespace ContinuousRunner
{
    using Data;

    public interface IResultWriter
    {
        void Write(IEnumerable<TestSuite> testSuites);
    }
}
