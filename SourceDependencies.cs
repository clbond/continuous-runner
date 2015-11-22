using System.Linq;
using System.Collections.Generic;
using System.IO;
using Magnum.Extensions;

namespace TestRunner
{
    public class SourceDependencies : ISourceDependencies
    {
        #region Private members

        private readonly ISet<IScript> _set = new SortedSet<IScript>();

        private readonly IDictionary<string, IScript> _map = new Dictionary<string, IScript>(Constants.EstimatedFiles);

        #endregion

        public void Add(IScript script)
        {
            _set.Add(script);
            _map.Add(script.File.Name, script);
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

        public IEnumerable<IScript> GetDependencies(IScript origin)
        {
            yield break;
        } 
    }
}
