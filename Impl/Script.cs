﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace ContinuousRunner.Impl
{
    using Data;

    public class Script : IScript
    {
        #region Constructors

        public Script(
            [NotNull] Func<IScript, SyntaxTree> reloader,
            [NotNull] Func<IScript, SyntaxTree, ModuleDefinition> moduleLoader,
            [NotNull] Func<IScript, SyntaxTree, IEnumerable<TestSuite>> suiteLoader)
        {
            _reloader = reloader;

            _moduleLoader = moduleLoader;

            _suiteLoader = suiteLoader;

            Reset();
        }

        #endregion

        #region Private members

        private readonly Func<IScript, SyntaxTree> _reloader;

        private readonly Func<IScript, SyntaxTree, ModuleDefinition> _moduleLoader;

        private readonly Func<IScript, SyntaxTree, IEnumerable<TestSuite>> _suiteLoader;

        private Lazy<IEnumerable<TestSuite>> _suites;

        private Lazy<IEnumerable<IScript>> _dependencies;

        private SyntaxTree _syntaxTree;

        #endregion

        #region Implementation of IScript

        public FileInfo File { get; set; }

        public ModuleDefinition Module { get; private set; }

        public IEnumerable<TestSuite> Suites => _suites.Value;

        public IEnumerable<IScript> Requires => _dependencies.Value;

        public string Content { get; set; }

        public string Description => File?.Name ?? @"anonymous script";

        public int TestCount
        {
            get
            {
                return Suites.Sum(suite => suite.Tests.Count);
            }
        }

        public SyntaxTree SyntaxTree
        {
            get
            {
                if (_syntaxTree == null)
                {
                    SyntaxTree = _reloader?.Invoke(this);
                }

                return _syntaxTree;
            }
            set
            {
                Reset();

                _syntaxTree = value;

                // When our syntax tree changes -- i.e., when the file has been loaded, or it has been
                // changed and the change detected by the file watcher -- then we need to search through
                // the code again to extract the define() call and determine what the dependencies of
                // this file are so that we can construct a dependency tree.
                Module = _moduleLoader?.Invoke(this, value);

                // Let observers know that we have changed
                Changed?.Invoke(this);
            }
        }
        
        public void Reload()
        {
            SyntaxTree = _reloader?.Invoke(this);

            if (SyntaxTree == null)
            {
                throw new TestException($"Reload failed: reloader delegate returned null");
            }
        }

        public event SourceChangedHandler Changed;

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

            _dependencies = new Lazy<IEnumerable<IScript>>(GetDependencies);
        }

        private IEnumerable<IScript> GetDependencies()
        {
            return Module.GetDependencies();
        }

        private IEnumerable<TestSuite> GetSuites()
        {
            return _suiteLoader?.Invoke(this, SyntaxTree);
        }

        #endregion
    }
}