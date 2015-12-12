using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContinuousRunner.Extractors;
using JetBrains.Annotations;

namespace ContinuousRunner.Impl
{
    using Data;

    public class Script : IScript
    {
        #region Constructors

        public Script(
            [NotNull] IPublisher publisher,
            [NotNull] Func<IScript, ExpressionTree> reloader,
            [NotNull] Func<IScript, ExpressionTree, ModuleDefinition> moduleLoader,
            [NotNull] Func<IScript, ExpressionTree, Definer> suiteLoader)
        {
            _publisher = publisher;

            _reloader = reloader;

            _moduleLoader = moduleLoader;

            _suiteLoader = suiteLoader;

            Reset();
        }

        #endregion

        #region Private members

        private readonly IPublisher _publisher;

        private readonly Func<IScript, ExpressionTree> _reloader;

        private readonly Func<IScript, ExpressionTree, ModuleDefinition> _moduleLoader;

        private readonly Func<IScript, ExpressionTree, Definer> _suiteLoader;

        private Lazy<IEnumerable<TestSuite>> _suites;
        
        private ExpressionTree _expressionTree;

        #endregion

        #region Implementation of IScript

        public FileInfo File { get; set; }

        public ModuleDefinition Module { get; private set; }

        public IEnumerable<TestSuite> Suites => _suites.Value;
        
        public string Content { get; set; }

        public string Description => File?.Name ?? @"anonymous script";

        public int TestCount
        {
            get
            {
                return Suites.Sum(suite => suite.Tests.Count);
            }
        }

        public ExpressionTree ExpressionTree
        {
            get
            {
                if (_expressionTree == null)
                {
                    ExpressionTree = _reloader?.Invoke(this);
                }

                return _expressionTree;
            }
            set
            {
                Reset();

                _expressionTree = value;

                // When our syntax tree changes -- i.e., when the file has been loaded, or it has been
                // changed and the change detected by the file watcher -- then we need to search through
                // the code again to extract the define() call and determine what the dependencies of
                // this file are so that we can construct a dependency tree.
                Module = _moduleLoader?.Invoke(this, value);

                // Notify observers that the source file has chnaged
                _publisher.Publish(
                    new SourceChangedEvent
                    {
                        Operation = ContinuousRunner.Operation.Change,
                        Script = this
                    });
            }
        }
        
        public void Reload()
        {
            ExpressionTree = _reloader?.Invoke(this);

            if (ExpressionTree == null)
            {
                throw new TestException($"Reload failed: reloader delegate returned null");
            }
        }
        
        #endregion

        #region Implementation of IComparable<in IScript>

        public int CompareTo(IScript other)
        {
            return string.Compare(File.Name, other.File.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Private methods

        private void Reset()
        {
            _suites = new Lazy<IEnumerable<TestSuite>>(GetSuites);
        }

        private IEnumerable<TestSuite> GetSuites()
        {
            var definer = _suiteLoader?.Invoke(this, ExpressionTree);
            if (definer != null)
            {
                return definer.GetSuites();
            }

            return Enumerable.Empty<TestSuite>();
        }

        #endregion
    }
}