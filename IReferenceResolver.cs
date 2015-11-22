namespace TestRunner
{
    public interface IReferenceResolver
    {
        /// <summary>
        /// Resolve a require statement into an absolute module reference ID. So for example, if we
        /// attempt to resolve ./Baz from Base/Bar.js, then it will resolve into Base/Baz, which is
        /// an absolute representation of the same module ID.
        /// </summary>
        string Resolve(IScript script, string require);
    }
}
