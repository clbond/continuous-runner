using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContinuousRunner.Frameworks;
using JetBrains.Annotations;
using Jint.Parser.Ast;

namespace ContinuousRunner.Impl
{
    using Data;

    public class Script : IScript
    {
        #region Constructors

        public Script(
            [NotNull] IPublisher publisher,
            [NotNull] Func<IScript, ExpressionTree<SyntaxNode>> reloader,
            [NotNull] Func<IScript, ExpressionTree<SyntaxNode>, ModuleDefinition> moduleLoader,
            [NotNull] Func<IScript, ExpressionTree<SyntaxNode>, ITestCollection> suiteLoader,
            [NotNull] Func<IProjectSource, Framework> frameworkLoader)
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

        private readonly Func<IScript, ExpressionTree<SyntaxNode>> _reloader;

        private readonly Func<IScript, ExpressionTree<SyntaxNode>, ModuleDefinition> _moduleLoader;

        private readonly Func<IScript, ExpressionTree<SyntaxNode>, ITestCollection> _suiteLoader;

        private readonly Func<IProjectSource, Framework> _frameworkLoader;

        private Lazy<IEnumerable<TestSuite>> _suites;

        private Lazy<Framework> _frameworks;

        private Lazy<ModuleDefinition> _module;

        private ExpressionTree<SyntaxNode> _expressionTree;

        #endregion

        #region Implementation of IScript

        public FileInfo File { get; set; }

        public ModuleDefinition Module => _module.Value;

        public IEnumerable<TestSuite> Suites => _suites.Value;

        public string Content { get; set; }

        public string Description => File?.Name ?? @"anonymous script";

        public int TestCount
        {
            get { return Suites.Sum(suite => suite.Tests.Count); }
        }

        public Framework Frameworks => _frameworks.Value;

        public IList<Tuple<DateTime, Severity, string>> Logs { get; private set; }

        public ExpressionTree<SyntaxNode> ExpressionTree
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

        public int CompareTo(IProjectSource other)
        {
            return string.Compare(File.Name, other.File.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Overrides of System.Object

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Description))
            {
                return Description;
            }

            if (!string.IsNullOrEmpty(File?.Name))
            {
                return File.Name;
            }

            if (!string.IsNullOrEmpty(Module?.ModuleName))
            {
                return Module.ModuleName;
            }

            return @"anonymous script";
        }

        #endregion

        #region Private methods

        private void Reset()
        {
            _suites = new Lazy<IEnumerable<TestSuite>>(GetSuites);

            _frameworks = new Lazy<Framework>(() => _frameworkLoader.Invoke(this));

            _module = new Lazy<ModuleDefinition>(() => _moduleLoader?.Invoke(this, ExpressionTree));

            Logs = new List<Tuple<DateTime, Severity, string>>();
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