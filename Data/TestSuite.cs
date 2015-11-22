using System.Collections.Generic;

namespace TestRunner.Data
{
    public class TestSuite
    {
        public string Name { get; set; }

        public IList<ITest> Tests { get; set; }
    }
}