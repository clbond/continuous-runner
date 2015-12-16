using System.Collections.Generic;
using System.IO;
using Jint.Parser.Ast;

namespace ContinuousRunner.Frameworks.RequireJs
{
    public interface IConfigurationLoader
    {
        /// <summary>
        /// Look through all the files in <paramref name="search"/> to see which file contains the call to <code>requirejs.config()</code>,
        /// and then parse the config call into a structure that gives us some information about the location of various libraries and so
        /// forth. This is really expensive since we have to parse every file in the list, so the results of this search -- at the very least,
        /// which file contains the <code>requirejs.config()</code> call -- should be saved somewhere so that we do not need to repeatedly
        /// conduct this search. Again, it's a very expensive call so be wary.
        /// </summary>
        IRequireConfiguration Load(IEnumerable<FileInfo> search);

        /// <summary>
        /// Parse the contents of <paramref name="script"/> to determine if <code>requirejs.config()</code> is called, and if it is, parse
        /// that call into an <see cref="IRequireConfiguration"/> object.
        /// </summary>
        IEnumerable<IRequireConfiguration> Load(FileInfo fileInfo);

        /// <summary>
        /// Is the specified function call a <code>requirejs.config()</code> invocation?
        /// </summary>
        bool IsRequireConfigCall(CallExpression callExpression);
    }
}
