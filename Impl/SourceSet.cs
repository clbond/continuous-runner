using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Magnum;
using Magnum.Extensions;

namespace ContinuousRunner.Impl
{
    using Data;

    public class SourceSet : ISourceSet
    {
        #region Constructors

        public SourceSet(IRunQueue testQueue, IEnumerable<ISourceObserver> observers)
        {
            Guard.AgainstNull(testQueue, nameof(testQueue));
            _testQueue = testQueue;

            _observers = observers;
        }

        #endregion

        #region Private members

        private readonly ISet<IScript> _set = new SortedSet<IScript>();

        private readonly IDictionary<string, IScript> _map = new Dictionary<string, IScript>();

        private readonly IRunQueue _testQueue;

        private readonly IEnumerable<ISourceObserver> _observers;

        #endregion

        #region Implementation of ISourceSet

        public void Add(IScript script)
        {
            _set.Add(script);

            _map.Add(script.File.Name, script);

            Changed(script);

            NotifyObservers(o => o.Added(script));
        }

        public void Remove(IScript script)
        {
            _set.Remove(script);

            _map.Remove(script.File.Name);

            NotifyObservers(o => o.Removed(script));
        }

        public void Remove(FileInfo fileInfo)
        {
            var matches = _set.Where(s => s.File.FullName == fileInfo.FullName).ToList();

            matches.Each(Remove);
        }

        public void Changed(IScript script)
        {
            NotifyObservers(o => o.Changed(script));

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

        #endregion

        #region Private methods

        private void NotifyObservers(Action<ISourceObserver> handler)
        {
            if (_observers == null)
            {
                return;
            }

            foreach (var observer in _observers)
            {
                handler?.Invoke(observer);
            }
        }

        #endregion
    }
}