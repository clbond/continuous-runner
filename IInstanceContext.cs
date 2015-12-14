namespace ContinuousRunner
{
    using System.IO;

    public interface IInstanceContext
    {
        /// <summary>
        /// The root of the entire solution
        /// </summary>
        DirectoryInfo SolutionRoot { get; }

        /// <summary>
        /// This is the path to all of our TypeScript and JavaScript source code. This should not be the root
        /// of the entire project, but the root of the client-side scripts. Typically this will be a folder
        /// called Scripts, but not always. Basically this should align with the directory that your script
        /// files reference when they import modules.
        /// </summary>
        DirectoryInfo ScriptsRoot { get; }

        /// <summary>
        /// The primary namespace that the TypeScript code in this project is organized under
        /// </summary>
        string ModuleNamespace { get; }
    }
}
