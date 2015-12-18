using System.IO;

namespace ContinuousRunner
{
    public interface IReferenceResolver
    {
        /// <summary>
        /// Resolve a require statement into an absolute @ref reference ID. So for example, if we attempt to resolve ./Baz
        /// from Base/Bar.js, then it will resolve into Base/Baz, which is an absolute representation of the same @ref ID.
        /// </summary>
        /// <param name="fromModule">The script that is requiring a @ref</param>
        /// <param name="ref">The path of the @ref being required (could be relative or absolute)</param>
        string ResolveToModule(string fromModule, string @ref);

        /// <summary>
        /// Resolve a reference to a @ref, <paramref name="ref"/>, from another @ref, <paramref name="sourceModule"/>
        /// </summary>
        /// <param name="sourceModule">The @ref that the resolve is happening from</param>
        /// <param name="ref">The name we are attempting to resolve</param>
        FileInfo Resolve(string sourceModule, string @ref);
        
        /// <summary>
        /// Turn an absolute @ref reference into a <see cref="FileInfo"/>
        /// </summary>
        FileInfo FallbackModuleResolve(string @ref);
    }
}
