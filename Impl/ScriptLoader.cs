using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContinuousRunner.Extractors;
using JetBrains.Annotations;

using Magnum;

using NLog;

namespace ContinuousRunner.Impl
{
    using Data;

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
            string content;
            using (var readStream = script.OpenRead())
            {
                using (var sr = new StreamReader(readStream))
                {
                    content = sr.ReadToEnd();
                }
            }

            return LoadScript(content, script);
        }

        public IScript Load(string content)
        {
            return LoadScript(content, null);
        }

        #endregion

        #region Private methods

        private IScript TryLoad(FileInfo fileInfo)
        {
            try
            {
                _logger.Info($"Loading script: {0}", fileInfo.Name);

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
                _logger.Error(ex, $"Failed to load script: {fileInfo.FullName}");

                return null;
            }
        }

        private IScript LoadScript(string content, FileInfo fileInfo)
        {
            Func<IScript, SyntaxTree> loader = s => _parser.Parse(content);

            Func<IScript, SyntaxTree, ModuleDefinition> moduleLoader = (s, tree) => _moduleReader.Get(s);

            Func<IScript, SyntaxTree, IEnumerable<TestSuite>> suiteLoader = (s, tree) => _suiteReader.GetTests(s);

            var syntaxTree = fileInfo != null
                                 ? _parser.Parse(fileInfo)
                                 : _parser.Parse(content);

            return new Script(loader, moduleLoader, suiteLoader)
            {
                File = fileInfo,
                Content = content,
                SyntaxTree = syntaxTree
            };

        }

        #endregion
    }
}