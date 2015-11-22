using System.Collections.Generic;

namespace TestRunner
{
    using Data;

    public interface IResultWriter
    {
        void Write(IEnumerable<TestSuite> testSuites);
    }
}
