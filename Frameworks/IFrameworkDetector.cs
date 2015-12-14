using System.Collections.Generic;

using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Frameworks
{
    public interface IFrameworkDetector
    {
        /// <summary>
        /// Determine what probable frameworks the specified script, <paramref name="script"/>, is using. We have a set
        /// of heuristics that we apply to a piece of code to determine what frameworks it is using, but these rules are
        /// not totally reliable, so there is a way for the user to specify exactly what frameworks he is using by
        /// configuring <see cref="IInstanceContext" />.
        /// </summary>
        Framework DetectFrameworks(IScript script);

        /// <summary>
        /// Determine what frameworks are probably being used, but use several source files as input instead of a single
        /// source file. This is more reliable; you should use this overload whenever possible.
        /// </summary>
        Framework DetectFrameworks(IEnumerable<IScript> script);

        /// <summary>
        /// Install the appropriate frameworks (<paramref name="framework"/> flags) into a V8 script context.
        /// </summary>
        /// <param name="script">The script we are preparing to execute in this V8 context</param>
        /// <param name="framework">The frameworks to install into the V8 script context</param>
        /// <param name="engine">An existing V8 script context</param>
        void InstallFrameworks(IScript script, Framework framework, V8ScriptEngine engine);
    }
}
