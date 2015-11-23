using System;
using System.Collections.Generic;
using System.Linq;
using Magnum;
using NLog;
using System.IO;
using ContinuousRunner.Data;
using JetBrains.Annotations;

namespace ContinuousRunner.Impl
{
    public class ScriptLoader : IScriptLoader
    {
        #region Constructors

        public ScriptLoader(
            [NotNull] IInstanceContext instanceContext,
            [NotNull] IModuleReader moduleReader,
            [NotNull] IParser parser,
            [NotNull] ISourceMutator sourceMutator,
            [NotNull] ISourceSet sourceSet,
            [NotNull] ISuiteReader suiteReader)
        {
            Guard.AgainstNull(instanceContext, nameof(instanceContext));
            Guard.AgainstNull(moduleReader, nameof(moduleReader));
            Guard.AgainstNull(parser, nameof(parser));
            Guard.AgainstNull(sourceMutator, nameof(sourceMutator));
            Guard.AgainstNull(sourceSet, nameof(sourceSet));
            Guard.AgainstNull(suiteReader, nameof(suiteReader));

            _instanceContext = instanceContext;
            _moduleReader = moduleReader;
            _parser = parser;
            _sourceMutator = sourceMutator;
            _sourceSet = sourceSet;
            _suiteReader = suiteReader;
        }

        #endregion

        #region Private members

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IInstanceContext _instanceContext;

        private readonly IModuleReader _moduleReader;

        private readonly IParser _parser;

        private readonly ISourceSet _sourceSet;

        private readonly ISourceMutator _sourceMutator;

        private readonly ISuiteReader _suiteReader;

        #endregion


        #region Implementation of IScriptLoader

        public IEnumerable<IScript> GetScripts()
        {
            var files = _instanceContext.ScriptsRoot.GetFiles(Constants.FilenameFilter, SearchOption.AllDirectories);

            return files.Select(TryLoad).Where(script => script != null);
        }

        public IEnumerable<IScript> GetTestScripts()
        {
            var files = _instanceContext.ScriptsRoot.GetFiles(Constants.FilenameFilter, SearchOption.AllDirectories);

            files = files.Where(f => Constants.SearchExpression.IsMatch(f.Name)).ToArray();

            return files.Select(TryLoad).Where(s => s != null);
        }

        public IScript Load(FileInfo script)
        {
            Func<IScript, SyntaxTree> loader = s => _parser.Parse(s.File);

            Func<IScript, SyntaxTree, ModuleDefinition> moduleLoader = (s, tree) => _moduleReader.Get(s);

            Func<IScript, SyntaxTree, IEnumerable<TestSuite>> suiteLoader = (s, tree) => _suiteReader.Get(s);

            return new Script(loader, moduleLoader, suiteLoader)
                   {
                       File = script,
                       SyntaxTree = _parser.Parse(script)
                   };
        }

        #endregion

        #region Private methods

        private IScript TryLoad(FileInfo fileInfo)
        {
            try
            {
                _logger.Debug($"Loading script: {0}", fileInfo.Name);

                var script = _sourceSet.GetScript(fileInfo);
                if (script == null)
                {
                    script = Load(fileInfo);
                }
                else
                {
                    script.Reload();
                }

                _sourceMutator.Add(script);

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