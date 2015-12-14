using System;
using System.ComponentModel.Composition;
using System.IO;

using ContinuousRunner.Frameworks;
using Jint.Parser.Ast;
using NLog;

namespace ContinuousRunner.Impl
{
    using Data;

    public class ScriptLoader : IScriptLoader
    {
        #region Private members

        [Import] private readonly ICachedScripts _cachedScripts;

        [Import] private readonly IFrameworkDetector _frameworkDetector;

        [Import] private readonly IModuleReader _moduleReader;

        [Import] private readonly IParser<SyntaxNode> _parser;

        [Import] private readonly IPublisher _publisher;

        [Import] private readonly ITestCollectionReader _suiteReader;

        [Import] private readonly IReferenceResolver _referenceResolver;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Implementation of IScriptLoader

        public IScript Load(FileInfo script)
        {
            return _cachedScripts.Get(script, LoadFile);
        }

        public IScript TryLoad(FileInfo fileInfo)
        {
            try
            {
                return Load(fileInfo);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to load script: {fileInfo.FullName}");

                return null;
            }
        }

        public IScript Load(string content)
        {
            var logger = LogManager.GetCurrentClassLogger();

            logger.Debug($"Loading script: [{content}]");

            return LoadScript(content, null);
        }

        public IScript LoadModule(IScript fromScript, string relativeReference)
        {
            var logger = LogManager.GetCurrentClassLogger();

            var absoluteReference = _referenceResolver.Resolve(fromScript, relativeReference);
            if (absoluteReference == null)
            {
                logger.Error($"Unable to resolve reference: {fromScript} -> {relativeReference}");
                return null;
            }

            return LoadModule(absoluteReference);
        }

        public IScript LoadModule(string absoluteReference)
        {
            var logger = LogManager.GetCurrentClassLogger();

            var fileInfo = _referenceResolver.ModuleReferenceToFile(absoluteReference);
            if (fileInfo.Exists == false)
            {
                logger.Error($"Referenced file does not exist: {absoluteReference} -> {fileInfo}");

                return null;
            }

            return TryLoad(fileInfo);
        }

        #endregion

        #region Private methods

        private IScript LoadScript(string content, FileInfo fileInfo)
        {
            Func<IScript, ExpressionTree<SyntaxNode>> loader = s => _parser.Parse(content);

            Func<IScript, ExpressionTree<SyntaxNode>, ModuleDefinition> moduleLoader =
                (s, tree) => _moduleReader.Get(s, m => LoadModule(s, m));

            Func<IScript, ExpressionTree<SyntaxNode>, ITestCollection> suiteLoader = (s, tree) => _suiteReader.DefineTests(s);

            Func<IProjectSource, Framework> frameworkLoader = s => _frameworkDetector.DetectFrameworks(s);

            var expressionTree = fileInfo != null
                                     ? _parser.Parse(fileInfo)
                                     : _parser.Parse(content);

            var script = new Script(_publisher, loader, moduleLoader, suiteLoader, frameworkLoader)
                         {
                             File = fileInfo,
                             Content = content,
                             ExpressionTree = expressionTree
                         };

            _publisher.Publish(
                new SourceChangedEvent
                {
                    Operation = Operation.Add,
                    Script = script
                });

            return script;
        }

        private IScript LoadFile(FileInfo fileInfo)
        {
            var logger = LogManager.GetCurrentClassLogger();

            logger.Debug($"Loading script file: {fileInfo.Name}");

            string content;

            using (var readStream = fileInfo.OpenRead())
            {
                using (var sr = new StreamReader(readStream))
                {
                    content = sr.ReadToEnd();
                }
            }

            return LoadScript(content, fileInfo);
        }

        #endregion
    }
}