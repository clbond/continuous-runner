using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContinuousRunner.Frameworks;
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
            [NotNull] Func<IScript, ExpressionTree, ITestCollection> suiteLoader,
            [NotNull] Func<IScript, Framework> frameworkLoader)
        {
            _publisher = publisher;

            _reloader = reloader;

            _moduleLoader = moduleLoader;

            _suiteLoader = suiteLoader;

            _frameworkLoader = frameworkLoader;

            Reset();
        }

        #endregion

        #region Private members

        private readonly IPublisher _publisher;

        private readonly Func<IScript, ExpressionTree> _reloader;

        private readonly Func<IScript, ExpressionTree, ModuleDefinition> _moduleLoader;

        private readonly Func<IScript, ExpressionTree, ITestCollection> _suiteLoader;

        private Lazy<IEnumerable<TestSuite>> _suites;

        private Lazy<Framework> _frameworks;

        private ExpressionTree _expressionTree;

        private Func<IScript, Framework> _frameworkLoader;

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

        public Framework Frameworks => _frameworks.Value;

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

        #region Overrides of System.Object

        #region Overrides of Object

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Description))
            {
                return Description;
            }

            if (File != null)
            {
                return File.Name;
            }

            if (Module != null)
            {
                return Module.ModuleName;
            }

            return @"anonymous script";
        }

        #endregion

        #endregion

        #region Private methods

        private void Reset()
        {
            _suites = new Lazy<IEnumerable<TestSuite>>(GetSuites);

            _frameworks = new Lazy<Framework>(() => _frameworkLoader.Invoke(this));
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