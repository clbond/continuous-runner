using System;
using System.Text.RegularExpressions;
using Magnum.Extensions;

namespace ContinuousRunner
{
    public class Constants
    {
        public static readonly TimeSpan TestTimeout = 5.Minutes(); // timeout of complete test run

        public const uint MaximumQueueSize = 8;

        public static readonly TimeSpan QueueWait = 150.Milliseconds();

        /// <summary>
        /// The expression we will use to search for TypeScript tests
        /// </summary>
        public static readonly Regex SearchExpression = new Regex("(spec|tests|test).js$",
                                                                  RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// An overly-broad string to narrow the list of files that we care about watching (but the regular expression above
        /// is the final word on whether a file will be considered for inclusion in the watch set, not this filter; this
        /// is just a performance optimization).
        /// </summary>
        public const string FilenameFilter = @"*.js";

        public static class FunctionIdentifiers
        {
            /// <summary>
            /// The name of the JavaScript function that is used to define a test suite (eg describe())
            /// </summary>
            public static readonly string[] SuiteFunctions = {@"describe", @"fdescribe"};

            /// <summary>
            /// The name of the JavaScript function that is used to define a test
            /// </summary>
            public static readonly string[] TestFunction = {@"it", @"fit"};

            public static readonly string[] RequireFunctions = {"require", "requirejs", "define"};
        }
    }
}