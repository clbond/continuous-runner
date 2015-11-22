using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace TestRunner
{
    public class BackgroundWatcher : IBackgroundWatcher
    {
        #region Private members

        private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        private readonly FileSystemWatcher _watcher = new FileSystemWatcher();

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Implementation of IBackgroundRunner

        public async void Watch(DirectoryInfo path)
        {
            await Task.Run(() => Start(path), _cancellationToken.Token);
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;

            _cancellationToken.Cancel();
        }

        private Task Start(DirectoryInfo path)
        {
            var tsc = new TaskCompletionSource<int>();

            _watcher.BeginInit();

            _watcher.Path = path.FullName;
            _watcher.EnableRaisingEvents = true;
            _watcher.Filter = Constants.FileFilter; // .ts, .js probably

            _watcher.Error += OnError;
            _watcher.Changed += OnChanged;
            _watcher.Created += OnCreated;
            _watcher.Deleted += OnDeleted;
            _watcher.Renamed += OnRenamed;

            _watcher.EndInit();

            return tsc.Task;
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _cancellationToken.Dispose();

            _watcher.Dispose();
        }

        #endregion

        #region Private methods

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            _logger.Error(e.GetException(), @"Error while watching for filesystem changes");

            Stop();
        }

        #endregion
    }
}
