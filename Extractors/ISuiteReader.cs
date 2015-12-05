using System.Collections.Generic;

namespace ContinuousRunner.Extractors
{
    using Data;

    public interface ISuiteReader
    {
        IEnumerable<TestSuite> GetTests(IScript script);
    }
}
