using System;
using System.Threading;

namespace ContinuousRunner.Console
{
    public class ConsoleReader : IDisposable
    {
        private static readonly AutoResetEvent _read = new AutoResetEvent(false);

        private static Thread _thread;

        private int _lastRead;

        public void Start()
        {
            _thread = new Thread(Run) { IsBackground = true };
            _thread.Start();
        }

        private void Run()
        {
            while (true)
            {
                _lastRead = System.Console.Read();

                _read.Set();
            }
        }

        public char? Read(TimeSpan wait)
        {
            if (_read.WaitOne(Convert.ToInt32(wait.TotalMilliseconds)))
            {
                _read.Reset();

                return (char) _lastRead;
            }

            return null;
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            _thread?.Join(TimeSpan.FromMilliseconds(750d));
            _thread = null;
            _read.Dispose();
        }

        #endregion
    }
}
