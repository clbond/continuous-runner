using System.Collections.Generic;
using ContinuousRunner.Data;

namespace ContinuousRunner
{
    public interface ISuiteReader
    {
        IEnumerable<TestSuite> Get(IScript script);
    }
}
