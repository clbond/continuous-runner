using ContinuousRunner.Data;

using NLog;

namespace ContinuousRunner.Impl
{
    public class ResultObserver : ISubscription<TestResult>
    {
        #region Implementation of ISubscription<in TestResult>

        public void Handle(TestResult @event)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
