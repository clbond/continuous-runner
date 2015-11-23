using ContinuousRunner.Extensions;
using NLog;

namespace ContinuousRunner.Impl
{
    public class ResultObserver : IResultObserver
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region Implementation of IResultObserver

        public void ResultChanged(TestResultChanged changedEvent)
        {
            _logger.Info($"Test result changed: {changedEvent}");

            var exceptions = OnResultChanged.SafeInvoke(changedEvent);

            foreach (var exception in exceptions)
            {
                _logger.Error(exception,
                              $"Error while processing test result state change: {changedEvent}: {exception.Message}");
            }
        }

        public event TestResultChangedHandler OnResultChanged;

        #endregion
    }
}
