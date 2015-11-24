using System.Collections.Generic;
using ContinuousRunner.Frameworks;

namespace ContinuousRunner
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
    }
}
