using System;
using NLog;

namespace ContinuousRunner.Impl
{
    public class SourceObserver : ISourceObserver
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region Implementation of ISourceObserver
        
        #endregion

        #region Implementation of ISourceObserver

        public void Added(IScript script)
        {
            SafeInvoke(OnAdded, script);
        }

        public event SourceChangedHandler OnAdded;

        public void Removed(IScript script)
        {
            SafeInvoke(OnRemoved, script);
        }

        public event SourceChangedHandler OnRemoved;

        public void Changed(IScript script)
        {
            SafeInvoke(OnChanged, script);
        }

        public event SourceChangedHandler OnChanged;

        #endregion

        #region Private methods

        /// <summary>
        /// We do not want an exception thrown from one event handler to impact subsequent handlers, so we
        /// wrap each handler invocation in a try-catch block so that we can at least ensure each handler is
        /// called, regardless of the success or failure of prior handlers
        /// </summary>
        private void SafeInvoke(SourceChangedHandler sourceChangedHandler, IScript script)
        {
            if (sourceChangedHandler != null)
            {
                foreach (var invoke in sourceChangedHandler.GetInvocationList())
                {
                    try
                    {
                        invoke.DynamicInvoke(script);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Uncaught exception while processing script add event: {ex.Message}");
                    }
                }
            }
        }

        #endregion
    }
}
