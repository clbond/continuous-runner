using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContinuousRunner.Impl
{
    using Data;

    public class RunQueue : IRunQueue
    {
        private readonly Queue<IScript> _queue = new Queue<IScript>();
         
        #region Implementation of ITestQueue

        public void Push(IScript script)
        {
            if (_queue.Contains(script) == false) // no sense in adding the same script twice
            {
                _queue.Enqueue(script);
            }
        }

        public IEnumerable<Task<TestResult>> Run()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
