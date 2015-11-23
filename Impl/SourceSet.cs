// ReSharper disable PossibleMultipleEnumeration

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Magnum;
using Magnum.Extensions;

namespace ContinuousRunner.Impl
{
    using Data;

    public class SourceSet : ISourceSet, ISourceMutator
    {
        #region Constructors

        public SourceSet(IRunQueue testQueue, IEnumerable<ISourceObserver> observers)
        {
            Guard.AgainstNull(testQueue, nameof(testQueue));
            _testQueue = testQueue;

            Guard.AgainstNull(observers, nameof(observers));
            _observers = observers;
        }

        #endregion

        #region Private members

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private readonly ISet<IScript> _set = new SortedSet<IScript>();

        private readonly IDictionary<string, IScript> _map = new Dictionary<string, IScript>();

        private readonly IRunQueue _testQueue;

        private readonly IEnumerable<ISourceObserver> _observers;

        #endregion

        #region Implementation of ISourceSet

        public void Add(IScript script)
        {
            _lock.EnterWriteLock();
            try
            {
                _set.Add(script);

                _map.Add(script.File.Name, script);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            NotifyObservers(o => o.Added(script));

            QueueRun(script);
        }

        public void Remove(IScript script)
        {
            _lock.EnterWriteLock();
            try
            {
                _set.Remove(script);
                _map.Remove(script.File.Name);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            NotifyObservers(o => o.Removed(script));
        }

        public void Remove(FileInfo fileInfo)
        {
            _lock.EnterReadLock();

            IList<IScript> matches;
            try
            {
                matches = _set.Where(s => s.File.FullName == fileInfo.FullName).ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }

            matches.Each(Remove);

            matches.Each(QueueRun);
        }

        public void Changed(IScript script)
        {
            NotifyObservers(o => o.Changed(script));

            QueueRun(script);
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
            yield return origin;

            foreach (var additionalRequire in origin.Requires.SelectMany(GetDependencies))
            {
                yield return additionalRequire;
            }
        }

        public IEnumerable<TestSuite> GetSuites()
        {
            return _set.SelectMany(script => script.Suites);
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _lock.Dispose();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Queue a re-run of all tests in <paramref name="script"/> as well as its entire dependency tree
        /// </summary>
        /// <param name="script">The script that has changed and requires a re-execution of its associated tests</param>
        private void QueueRun(IScript script)
        {
            _testQueue.Push(script);

            GetDependencies(script).Each(dependency => _testQueue.Push(dependency));
        }

        private void NotifyObservers(Action<ISourceObserver> handler)
        {
            if (_observers == null)
            {
                return;
            }

            _lock.EnterReadLock();
            try
            {
                foreach (var observer in _observers)
                {
                    handler?.Invoke(observer);
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #endregion
    }
}