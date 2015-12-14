using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp;

namespace ContinuousRunner.Impl
{
    using Data;
    using Frameworks;

    public class Class : IClass
    {
        public Class(
            [NotNull] IPublisher publisher,
            [NotNull] Func<IProjectSource, ExpressionTree<CSharpSyntaxNode>, IEnumerable<TestSuite>> suiteLoader,
            [NotNull] Func<IProjectSource, Framework> frameworkLoader)
        {
            _publisher = publisher;

            _suiteLoader = suiteLoader;

            _frameworkLoader = frameworkLoader;

            Reset();
        }

        #region Implementation of IComparable<in IProjectSource>

        public int CompareTo(IProjectSource other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private members

        private readonly IPublisher _publisher;

        private readonly Func<IProjectSource, ExpressionTree<CSharpSyntaxNode>, IEnumerable<TestSuite>> _suiteLoader;

        private readonly Func<IProjectSource, Framework> _frameworkLoader;

        private Lazy<IEnumerable<TestSuite>> _suites;

        private Lazy<Framework> _frameworks;

        #endregion

        #region Implementation of IProjectSource

        public FileInfo File { get; set; }

        public IEnumerable<TestSuite> Suites => _suites.Value;

        public string Content { get; set; }

        public string Description { get; set; }

        public int TestCount => Suites.SelectMany(s => s.Tests).Count();

        public Framework Frameworks => _frameworks.Value;

        public IList<Tuple<DateTime, Severity, string>> Logs { get; private set; }

        public void Reload()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IClass

        public ExpressionTree<CSharpSyntaxNode> ExpressionTree { get; set; }

        #endregion

        #region Private methods

        public void Reset()
        {
            Logs = new List<Tuple<DateTime, Severity, string>>();

            _suites = new Lazy<IEnumerable<TestSuite>>(() => _suiteLoader(this, ExpressionTree));

            _frameworks = new Lazy<Framework>(() => _frameworkLoader(this));
        }
        
        private IEnumerable<TestSuite> GetSuites()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
