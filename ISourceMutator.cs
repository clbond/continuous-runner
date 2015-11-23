using System.IO;

namespace ContinuousRunner
{
    public interface ISourceMutator
    {
        /// <summary>
        /// A new script has been added to the project
        /// </summary>
        void Add(IScript script);

        /// <summary>
        /// An existing script has been removed from the project
        /// </summary>
        /// <param name="script"></param>
        void Remove(IScript script);

        /// <summary>
        /// Remove an existing script by filename instead of <see cref="IScript" /> reference.
        /// </summary>
        void Remove(FileInfo fileInfo);

        /// <summary>
        /// An existing script has been changed or updated
        /// </summary>
        /// <param name="script"></param>
        void Changed(IScript script);
    }
}
