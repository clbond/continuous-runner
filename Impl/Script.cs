using System;
using System.Collections.Generic;
using System.IO;

namespace ContinuousRunner.Impl
{
    using Data;

    public class Script : IScript
    {
        private readonly IModuleReader _reader;

        private readonly IScriptParser _scriptParser;

        private Lazy<IEnumerable<TestSuite>> _suites;

        private Lazy<IEnumerable<IScript>> _dependencies; 

        private SyntaxTree _syntaxTree;

        public Script(IModuleReader reader, IScriptParser scriptParser)
        {
            _reader = reader;

            _scriptParser = scriptParser;
        }

        #region Implementation of IScript

        public FileInfo File { get; set; }

        public ModuleDefinition Module { get; private set; }

        public IEnumerable<TestSuite> Suites => _suites.Value;

        public IEnumerable<IScript> Requires => _dependencies.Value;

        public SyntaxTree SyntaxTree
        {
            get
            {
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
                Module = _reader.Get(this);
            }
        }

        public void Reload()
        {
            SyntaxTree = _scriptParser.Parse(File);
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

            _dependencies = new Lazy<IEnumerable<IScript>>(GetDependencies);
        }

        private IEnumerable<IScript> GetDependencies()
        {
            return Module.GetDependencies();
        }

        private IEnumerable<TestSuite> GetSuites()
        {
            yield break;
        }

        #endregion
    }
}