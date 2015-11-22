using System.Collections.Generic;

namespace TestRunner
{
    public class TestQueue : ITestQueue
    {
        #region Implementation of ITestQueue

        public void Push(IScript script)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<TestResult> Execute()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
