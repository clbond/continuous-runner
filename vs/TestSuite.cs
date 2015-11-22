using System.Collections.Generic;

namespace TestRunner
{
    public class TestSuite
    {
        public IList<ITest> Tests { get; private set; }
    }
}