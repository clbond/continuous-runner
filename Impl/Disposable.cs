using System;

namespace ContinuousRunner.Impl
{
    public class Disposable : IDisposable
    {
        private bool _isDisposed;

        private readonly Action _action;

        public Disposable(Action action)
        {
            _action = action;
        }

        ~Disposable()
        {
            Dispose();
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (_isDisposed == false)
            {
                _isDisposed = true;

                _action();
            }
        }

        #endregion
    }
}
