using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using ContinuousRunner.Extractors;

using NLog;

namespace ContinuousRunner.Impl
{
    using Data;

    public class ScriptLoader : IScriptLoader
    {
        #region Private members

        [Import] private readonly IInstanceContext _instanceContext;

        [Import] private readonly ICachedScripts _cachedScripts;

        [Import] private readonly IModuleReader _moduleReader;

        [Import] private readonly IParser _parser;
        
        [Import] private readonly IPublisher _publisher;

        [Import] private readonly ISuiteReader _suiteReader;

        [Import] private readonly IReferenceResolver _referenceResolver;

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
                var logger = LogManager.GetCurrentClassLogger();

                logger.Error(ex, $"Failed to load script: {fileInfo.FullName}");

                return null;
            }
        }

        public IScript Load(string content)
        {
            var logger = LogManager.GetCurrentClassLogger();

            logger.Debug($"Loading script: [{content}]");

            return LoadScript(content, null);
        }

        #endregion

        #region Private methods

        private IScript LoadScript(string content, FileInfo fileInfo)
        {
            Func<IScript, ExpressionTree> loader = s => _parser.Parse(content);

            Func<IScript, ExpressionTree, ModuleDefinition> moduleLoader =
                (s, tree) => _moduleReader.Get(s, m => LoadModule(s, m));

            Func<IScript, ExpressionTree, Definer> suiteLoader = (s, tree) => _suiteReader.Define(s);

            var expressionTree = fileInfo != null
                                     ? _parser.Parse(fileInfo)
                                     : _parser.Parse(content);

            return new Script(_publisher, loader, moduleLoader, suiteLoader)
                   {
                       File = fileInfo,
                       Content = content,
                       ExpressionTree = expressionTree
                   };

        }
        private IScript LoadFile(FileInfo fileInfo)
        {
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

        private IScript LoadModule(IScript fromScript, string moduleReference)
        {
            var logger = LogManager.GetCurrentClassLogger();

            var absoluteReference = _referenceResolver.Resolve(fromScript, moduleReference);
            if (absoluteReference == null)
            {
                logger.Error($"Unable to resolve reference: {fromScript} -> {moduleReference}");
                return null;
            }

            var fileInfo = ModuleReferenceToFile(absoluteReference);
            if (fileInfo.Exists == false)
            {
                logger.Error(
                    $"Resolved reference, but referenced file does not exist: {fromScript} -> {moduleReference} -> {fileInfo}");
                return null;
            }

            return TryLoad(fileInfo);
        }

        private FileInfo ModuleReferenceToFile(string @ref)
        {
            var root = _instanceContext.ScriptsRoot.FullName;

            var components = @ref.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            var path = Path.Combine(new List<string> { root }.Concat(components).ToArray());

            return new FileInfo(path);
        }

        #endregion
    }
}