using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestRunner
{
    public class RunQueue : IRunQueue
    {
        private readonly Queue<IScript> _queue = new Queue<IScript>();
         
        #region Implementation of ITestQueue

        public void Push(IScript script)
        {
            _queue.Enqueue(script);
        }

        public Task<IEnumerable<TestResult>> Run()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
