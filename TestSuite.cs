using System.Collections.Generic;

namespace TestRunner
{
    public class TestSuite
    {
        public string Name { get; set; }

        public IList<ITest> Tests { get; set; }
    }
}