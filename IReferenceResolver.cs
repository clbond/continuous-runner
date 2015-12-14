using System.IO;

namespace ContinuousRunner
{
    public interface IReferenceResolver
    {
        /// <summary>
        /// Resolve a require statement into an absolute module reference ID. So for example, if we attempt to resolve ./Baz
        /// from Base/Bar.js, then it will resolve into Base/Baz, which is an absolute representation of the same module ID.
        /// </summary>
        /// <param name="script">The script that is requiring a module</param>
        /// <param name="module">The path of the module being required (could be relative or absolute)</param>
        string Resolve(IScript script, string module);

        /// <summary>
        /// Resolve a reference to a module, <paramref name="module"/>, from another module, <paramref name="sourceModule"/>
        /// </summary>
        /// <param name="sourceModule">The module that the resolve is happening from</param>
        /// <param name="module">The name we are attempting to resolve</param>
        string Resolve(string sourceModule, string module);

        /// <summary>
        /// Turn an absolute module reference into a <see cref="FileInfo"/>
        /// </summary>
        FileInfo ModuleReferenceToFile(string absoluteReference);
    }
}
