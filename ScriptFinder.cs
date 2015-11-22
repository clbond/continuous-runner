using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;

namespace TestRunner
{
    public class ScriptFinder : IScriptFinder
    {
        #region Constructors

        public ScriptFinder(ISourceDependencies dependencies, IScriptLoader scriptLoader, Options options)
        {
            _dependencies = dependencies;

            _scriptLoader = scriptLoader;

            _options = options;
        }

        #endregion

        #region Private members

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly Options _options;

        private readonly IScriptLoader _scriptLoader;

        private readonly ISourceDependencies _dependencies;

        #endregion

        #region Implementation of IScriptFinder

        public IEnumerable<IScript> GetScripts()
        {
            var files = _options.Root.GetFiles(Constants.FileFilter, SearchOption.AllDirectories);

            return files.Select(TryLoad).Where(s => s != null).ToList();
        }

        public IEnumerable<IScript> GetTestScripts()
        {
            var files = _options.Root.GetFiles(Constants.FileFilter, SearchOption.AllDirectories);

            files = files.Where(f => Constants.SearchExpression.IsMatch(f.Name)).ToArray();

            return files.Select(TryLoad).Where(s => s != null).ToList();
        }

        #endregion

        #region Private methods

        private IScript TryLoad(FileInfo fileInfo)
        {
            try
            {
                var script = _dependencies.GetScript(fileInfo);
                if (script == null)
                {
                    script = _scriptLoader.Load(fileInfo);
                }

                return script;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to load script: {fileInfo.FullName}: {ex.Message}");

                return null;
            }
        }

        #endregion
    }
}
