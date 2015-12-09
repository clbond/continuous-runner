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
        [Import] private readonly IModuleReader _moduleReader;
        [Import] private readonly IParser _parser;
        [Import] private readonly IScriptCollection _scriptCollection;
        [Import] private readonly IPublisher _publisher;
        [Import] private readonly ISuiteReader _suiteReader;

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
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                logger.Info($"Loading script: {0}", fileInfo.Name);

                var script = _scriptCollection.FindFile(fileInfo);
                if (script == null)
                {
                    script = Load(fileInfo);
                }
                else
                {
                    script.Reload();
                }

                _publisher.Publish(
                    new SourceChangedEvent
                    {
                        Operation = ContinuousRunner.Operation.Add,
                        Script = script
                    });

                return script;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Failed to load script: {fileInfo.FullName}");

                return null;
            }
        }

        private IScript LoadScript(string content, FileInfo fileInfo)
        {
            Func<IScript, ExpressionTree> loader = s => _parser.Parse(content);

            Func<IScript, ExpressionTree, ModuleDefinition> moduleLoader = (s, tree) => _moduleReader.Get(s);

            Func<IScript, ExpressionTree, IEnumerable<TestSuite>> suiteLoader = (s, tree) => _suiteReader.GetTests(s);

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

        #endregion
    }
}