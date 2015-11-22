using System.Linq;
using System.Collections.Generic;
using System.IO;
using Magnum.Extensions;

namespace TestRunner
{
    public class SourceDependencies : ISourceDependencies
    {
        public SourceDependencies(IRunQueue testQueue)
        {
            _testQueue = testQueue;
        }

        #region Private members

        private readonly ISet<IScript> _set = new SortedSet<IScript>();

        private readonly IDictionary<string, IScript> _map = new Dictionary<string, IScript>(Constants.EstimatedFiles);

        private readonly IRunQueue _testQueue;

        #endregion

        public void Add(IScript script)
        {
            _set.Add(script);

            _map.Add(script.File.Name, script);

            Changed(script);
        }

        public void Remove(IScript script)
        {
            _set.Remove(script);

            _map.Remove(script.File.Name);
        }

        public void Remove(FileInfo fileInfo)
        {
            var matches = _set.Where(s => s.File.FullName == fileInfo.FullName).ToList();

            matches.Each(Remove);
        }

        public void Changed(IScript script)
        {
            _testQueue.Push(script);

            foreach (var dependency in GetDependencies(script))
            {
                _testQueue.Push(dependency);
            }
        }

        public IScript GetScript(FileInfo fileInfo)
        {
            if (_map.ContainsKey(fileInfo.FullName))
            {
                return _map[fileInfo.FullName];
            }

            return null;
        }

        public IScript GetScriptFromModuleReference(string absoluteReference)
        {
            return _set.FirstOrDefault(script => script.Module.ModuleName == absoluteReference);
        }

        public IEnumerable<IScript> GetDependencies(IScript origin)
        {
            yield break;
        }

        public IEnumerable<TestSuite> GetSuites()
        {
            return _set.SelectMany(script => script.Suites);
        }
    }
}