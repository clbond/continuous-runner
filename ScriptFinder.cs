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

        public ScriptFinder(IScriptLoader scriptLoader, Options options)
        {
            _scriptLoader = scriptLoader;

            _options = options;
        }

        #endregion

        #region Private members

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly Options _options;

        private readonly IScriptLoader _scriptLoader;

        #endregion

        #region Implementation of IScriptFinder

        public IEnumerable<IScript> GetScripts()
        {
            var root = new DirectoryInfo(_options.Path);

            var files = root.GetFiles(Constants.FileFilter, SearchOption.AllDirectories);

            files = files.Where(f => Constants.SearchExpression.IsMatch(f.Name)).ToArray();

            return files.Select(TryLoad).Where(s => s != null).ToList();
        }

        private IScript TryLoad(FileInfo fileInfo)
        {
            try
            {
                return _scriptLoader.Load(fileInfo);
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
