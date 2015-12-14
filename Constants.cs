using System;

using Magnum.Extensions;

namespace ContinuousRunner
{
    public class Constants
    {
        public static readonly TimeSpan TestTimeout = 5.Minutes(); // timeout of complete test run

        public static readonly TimeSpan QueueWait = 150.Milliseconds();

        /// <summary>
        /// The number of preloaded scripts to keep in memory in support of the test runner
        /// </summary>
        public const int ScriptCacheSize = 16;

        public static class FileExtensions
        {
            public static readonly string[] JavaScript = {@".js"};
            public static readonly string[] TypeScript = {@".ts"};
            public static readonly string[] CoffeeScript = {@".cs"};
        }

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