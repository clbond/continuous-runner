using System.Collections.Generic;

namespace TestRunner
{
    public interface IResultWriter
    {
        void Write(IEnumerable<TestSuite> testSuites);
    }
}
