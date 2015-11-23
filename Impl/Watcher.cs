using System;
using System.IO;
using JetBrains.Annotations;
using Magnum;
using NLog;

namespace ContinuousRunner.Impl
{
    public class Watcher : IWatcher
    {
        #region Constructors

        public Watcher(
            [NotNull] ISourceMutator mutator,
            [NotNull] ISourceSet sourceSet,
            [NotNull] IScriptLoader scriptLoader)
        {
            Guard.AgainstNull(mutator, nameof(mutator));
            _mutator = mutator;

            Guard.AgainstNull(sourceSet, nameof(sourceSet));
            _sourceSet = sourceSet;

            Guard.AgainstNull(scriptLoader, nameof(scriptLoader));
            _scriptLoader = scriptLoader;
        }

        #endregion

        #region Private members

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly ISourceSet _sourceSet;

        private readonly IScriptLoader _scriptLoader;

        private readonly ISourceMutator _mutator;

        #endregion

        #region Implementation of IBackgroundRunner
        
        public ICancellable Watch(DirectoryInfo scriptPath)
        {
            var watcher = new FileSystemWatcher(scriptPath.FullName);

            watcher.BeginInit();

            watcher.Path = scriptPath.FullName;
            watcher.EnableRaisingEvents = true;
            watcher.Filter = Constants.FilenameFilter; // .ts, .js probably

            watcher.Error += OnError;
            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;

            watcher.EndInit();

            return new Cancellable(() => watcher.Dispose());
        }

        #endregion

        #region Private methods

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            try
            {
                _mutator.Remove(new FileInfo(e.OldFullPath));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Cannot remove file from source set: {e.OldFullPath}: {ex.Message}");
            }

            try
            {
                var script = _scriptLoader.Load(new FileInfo(e.FullPath));

                _mutator.Add(script);
            }
            catch (Exception ex)
            {
                _logger.Error(ex,
                              $"Failed to load script file (file rename operation): {e.FullPath} (from {e.OldFullPath}): {ex.Message}");
            }
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                _mutator.Remove(new FileInfo(e.FullPath));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Cannot remove source file from dependency mapper: {ex.Message}");
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                var script = _scriptLoader.Load(new FileInfo(e.FullPath));

                _mutator.Add(script);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to load script file (file create operation): {e.FullPath}: {ex.Message}");
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                var script = _sourceSet.GetScript(new FileInfo(e.FullPath));
                if (script != null)
                {
                    script.Reload();

                    _mutator.Changed(script);
                }
                else
                {
                    script = _scriptLoader.Load(new FileInfo(e.FullPath));

                    _mutator.Add(script);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to load script file (file change operation): {e.FullPath}: {ex.Message}");
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            var exception = e.GetException();

            _logger.Error(exception, $"Error while watching for filesystem changes: {exception.Message}");
        }

        #endregion
    }
}