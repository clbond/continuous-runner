using System;

namespace ContinuousRunner.Impl
{
    public class Cancellable : ICancellable
    {
        private readonly Action _cancel;

        public Cancellable(Action cancel)
        {
            _cancel = cancel;
        }

        ~Cancellable()
        {
            Cancel();
        }

        #region Implementation of ICancellable

        public void Cancel()
        {
            _cancel?.Invoke();
        }

        #endregion
    }
}
