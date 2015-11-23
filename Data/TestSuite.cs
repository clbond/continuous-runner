using System.Collections.Generic;

namespace ContinuousRunner.Data
{
    public class TestSuite
    {
        public string Name { get; set; }

        public IList<ITest> Tests { get; set; }
    }
}