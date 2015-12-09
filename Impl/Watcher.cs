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
            [NotNull] IInstanceContext instanceContext,
            [NotNull] IPublisher publisher,
            [NotNull] IScriptCollection scriptCollection,
            [NotNull] IScriptLoader scriptLoader)
        {
            Guard.AgainstNull(instanceContext, nameof(instanceContext));
            _instanceContext = instanceContext;

            Guard.AgainstNull(publisher, nameof(publisher));
            _publisher = publisher;

            Guard.AgainstNull(scriptCollection, nameof(scriptCollection));
            _scriptCollection = scriptCollection;

            Guard.AgainstNull(scriptLoader, nameof(scriptLoader));
            _scriptLoader = scriptLoader;
        }

        #endregion

        #region Private members

        private readonly IInstanceContext _instanceContext;

        private readonly IScriptCollection _scriptCollection;

        private readonly IScriptLoader _scriptLoader;

        private readonly IPublisher _publisher;

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

        public ICancellable Watch()
        {
            return Watch(_instanceContext.ScriptsRoot);
        }

        #endregion

        #region Private methods

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Cannot remove file from source set: {e.OldFullPath}");
            }

            try
            {
                var existingScript = _scriptCollection.Find(s => s.File.FullName == e.OldFullPath);
                if (existingScript != null)
                {
                    _publisher.Publish(
                        new SourceChangedEvent
                        {
                            Operation = Operation.Remove,
                            Script = existingScript
                        });
                }

                var newScript = _scriptLoader.Load(new FileInfo(e.FullPath));
                if (newScript != null)
                {
                    _publisher.Publish(
                        new SourceChangedEvent
                        {
                            Operation = Operation.Add,
                            Script = newScript
                        });
                }
                else
                {
                    logger.Error($"Failed to load script: {e.FullPath} (unknown error)");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex,
                             $"Failed to load script file (file rename operation): {e.FullPath} (from {e.OldFullPath})");
            }
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                var existingScript = _scriptCollection.Find(s => s.File.FullName == e.FullPath);
                if (existingScript != null)
                {
                    _publisher.Publish(
                        new SourceChangedEvent
                        {
                            Operation = Operation.Remove,
                            Script = existingScript
                        });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Cannot remove source file from dependency mapper");
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                var script = _scriptLoader.Load(new FileInfo(e.FullPath));
                if (script != null)
                {
                    _publisher.Publish(
                        new SourceChangedEvent
                        {
                            Operation = Operation.Add,
                            Script = script
                        });
                }
            }
            catch (Exception ex)
            {
                var logger = LogManager.GetCurrentClassLogger();

                logger.Error(ex, $"Failed to load script file (file create operation): {e.FullPath}");
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                var script = _scriptCollection.Find(s => s.File.FullName == e.FullPath);

                script?.Reload();
            }
            catch (Exception ex)
            {
                var logger = LogManager.GetCurrentClassLogger();

                logger.Error(ex, $"Failed to load script file (file change operation): {e.FullPath}");
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            var exception = e.GetException();

            var logger = LogManager.GetCurrentClassLogger();

            logger.Error(exception, $"Error while watching for filesystem changes: {exception.Message}");
        }

        #endregion
    }
}