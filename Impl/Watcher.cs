using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace ContinuousRunner.Impl
{
    public class Watcher : IWatcher
    {
        #region Constructors

        public Watcher(ISourceDependencies dependencies, IScriptLoader scriptLoader, IRunQueue testQueue)
        {
            _dependencies = dependencies;

            _scriptLoader = scriptLoader;

            _testQueue = testQueue;
        }

        #endregion

        #region Private members

        private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        private readonly FileSystemWatcher _watcher = new FileSystemWatcher();

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ISourceDependencies _dependencies;

        private readonly IScriptLoader _scriptLoader;

        private readonly IRunQueue _testQueue;

        #endregion

        #region Implementation of IBackgroundRunner

        public DirectoryInfo Root
        {
            get
            {
                return new DirectoryInfo(_watcher.Path);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _watcher.Path = value.FullName;
            }
        }

        public async void Watch()
        {
            await Task.Run(() => Start(Root), _cancellationToken.Token);
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
            _watcher.Filter = Constants.FilenameFilter; // .ts, .js probably

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
            try
            {
                _dependencies.Remove(new FileInfo(e.OldFullPath));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Cannot remove source file from dependency mapper");
            }

            try
            {
                var script = _scriptLoader.Load(new FileInfo(e.FullPath));

                _dependencies.Add(script);
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                    "Failed to load script file (during rename operation): {0} (from {1})",
                    e.OldName,
                    e.OldFullPath);
            }
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                _dependencies.Remove(new FileInfo(e.FullPath));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Cannot remove source file from dependency mapper");
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                var script = _scriptLoader.Load(new FileInfo(e.FullPath));

                _dependencies.Add(script);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load script file (during rename operation): {0}", e.FullPath);
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                var script = _dependencies.GetScript(new FileInfo(e.FullPath));
                if (script != null)
                {
                    script.Reload();

                    _dependencies.Changed(script);
                }
                else
                {
                    script  = _scriptLoader.Load(new FileInfo(e.FullPath));

                    _dependencies.Add(script);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load script file (during rename operation): {0}", e.FullPath);
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            _logger.Error(e.GetException(), @"Error while watching for filesystem changes");

            Stop();
        }

        #endregion
    }
}
