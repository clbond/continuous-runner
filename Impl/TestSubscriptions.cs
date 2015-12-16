using System.ComponentModel.Composition;
using ContinuousRunner.Data;
using NLog;

namespace ContinuousRunner.Impl
{
    public class TestSubscriptions : ISubscription<SourceChangedEvent>,
                                     ISubscription<TestResult>
    {
        [Import] private readonly IRunQueue _runqueue;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region Implementation of ISubscription<in SourceChangedEvent>

        public void Handle(SourceChangedEvent @event)
        {
            _logger.Debug($"Source file changed: {@event}; queueing run");

            _runqueue.Push(@event.Script);
        }

        #endregion

        #region Implementation of ISubscription<in TestResult>

        public void Handle(TestResult @event)
        {
            var test = @event.Test;

            var s = test?.Suite?.ParentScript;
            if (s != null)
            {
                _logger.Debug($"Test result changed: {test}: {@event}");
            }
        }

        #endregion
    }
}
