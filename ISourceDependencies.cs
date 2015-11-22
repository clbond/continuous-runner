using System.Collections.Generic;
using System.IO;

namespace TestRunner
{
    public interface ISourceDependencies
    {
        void Add(IScript script);
        void Remove(IScript script);

        void Remove(FileInfo fileInfo);

        IEnumerable<IScript> GetDependencies(IScript origin);
    }
}