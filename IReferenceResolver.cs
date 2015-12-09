namespace ContinuousRunner
{
    public interface IReferenceResolver
    {
        /// <summary>
        /// Resolve a require statement into an absolute module reference ID. So for example, if we
        /// attempt to resolve ./Baz from Base/Bar.js, then it will resolve into Base/Baz, which is
        /// an absolute representation of the same module ID.
        /// </summary>
        /// <param name="script">The script that is requiring a module</param>
        /// <param name="module">The path of the module being required (could be relative or absolute)</param>
        string Resolve(IScript script, string module);
    }
}
