using ContinuousRunner.Extensions;
using NLog;

namespace ContinuousRunner.Impl
{
    public class SourceObserver : ISourceObserver
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        #region Implementation of ISourceObserver

        public void Added(IScript script)
        {
            var exceptions = OnAdded.SafeInvoke(script);

            foreach (var ex in exceptions)
            {
                _logger.Error(ex, $"Error while processing file-added event: {script.File.Name}");
            }
        }

        public event SourceChangedHandler OnAdded;

        public void Removed(IScript script)
        {
            var exceptions = OnRemoved.SafeInvoke(script);

            foreach (var ex in exceptions)
            {
                _logger.Error(ex, $"Error while processing file-removed event: {script.File.Name}");
            }
        }

        public event SourceChangedHandler OnRemoved;

        public void Changed(IScript script)
        {
            var exceptions = OnChanged.SafeInvoke(script);

            foreach (var ex in exceptions)
            {
                _logger.Error(ex, $"Error while processing source-changed event: {script.File.Name}");
            }
        }

        public event SourceChangedHandler OnChanged;

        #endregion
    }
}
