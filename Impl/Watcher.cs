using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

using NLog;

namespace ContinuousRunner.Impl
{
    public class Watcher : IWatcher
    {
        [Import] private readonly IInstanceContext _instanceContext;

        [Import] private readonly IPublisher _publisher;

        [Import] private readonly IScriptCollection _scriptCollection;

        [Import] private readonly ICachedScripts _scriptLoader;

        #region Implementation of IBackgroundRunner

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CC0022:Should dispose object", Justification = "Disose is called by consumer")]
        public IDisposable Watch(DirectoryInfo scriptPath)
        {
            var watcher = new FileSystemWatcher(scriptPath.FullName);

            watcher.BeginInit();

            watcher.Path = scriptPath.FullName;
            watcher.EnableRaisingEvents = true;

            watcher.Error += OnError;
            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;

            watcher.EndInit();

            return new Disposable(() => watcher.Dispose());
        }

        public IDisposable Watch()
        {
            return Watch(_instanceContext.ScriptsRoot);
        }

        #endregion

        #region Private methods

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (!IsScript(e.OldFullPath) && !IsScript(e.FullPath))
            {
                return;
            }

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
                var existingScript = _scriptCollection.FindScript(s => s.File.FullName == e.OldFullPath);
                if (existingScript != null)
                {
                    _publisher.Publish(
                        new SourceChangedEvent
                        {
                            Operation = Operation.Remove,
                            SourceFile = existingScript
                        });
                }

                var newScript = _scriptLoader.Load(new FileInfo(e.FullPath));
                if (newScript != null)
                {
                    _publisher.Publish(
                        new SourceChangedEvent
                        {
                            Operation = Operation.Add,
                            SourceFile = newScript
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
            if (!IsScript(e.FullPath))
            {
                return;
            }

            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                var existingScript = _scriptCollection.FindScript(s => s.File.FullName == e.FullPath);
                if (existingScript != null)
                {
                    _publisher.Publish(
                        new SourceChangedEvent
                        {
                            Operation = Operation.Remove,
                            SourceFile = existingScript
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
            if (!IsScript(e.FullPath))
            {
                return;
            }

            try
            {
                var script = _scriptLoader.Load(new FileInfo(e.FullPath));
                if (script != null)
                {
                    _publisher.Publish(
                        new SourceChangedEvent
                        {
                            Operation = Operation.Add,
                            SourceFile = script
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
            if (!IsScript(e.FullPath))
            {
                return;
            }

            try
            {
                var script = _scriptCollection.FindScript(s => s.File.FullName == e.FullPath);

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

        private static bool IsScript(string path)
        {
            Func<string, bool> isScript =
                ext => Constants.FileExtensions.TypeScript.Any(
                           t => string.Equals(ext, t, StringComparison.InvariantCultureIgnoreCase)) ||
                       Constants.FileExtensions.JavaScript.Any(
                           j => string.Equals(ext, j, StringComparison.InvariantCultureIgnoreCase));


            return isScript(Path.GetExtension(path));
        }

        private static bool IsScript(FileInfo fileInfo)
        {
            return IsScript(fileInfo.FullName);
        }

        #endregion
    }
}