using System;
using System.IO;
using CommandLine;

namespace TestRunner
{
    public class Options
    {
        #region Command-line arguments

        [Option('p', "path",
            Required = true,
            HelpText = "The path to search for JavaScript modules and tests")]
        public string Path { get; set; }

        #endregion

        #region Factory methods

        public static Options FromArgs(string[] args)
        {
            var options = new Options();

            if (CommandLine.Parser.Default.ParseArgumentsStrict(args, options, () => Environment.Exit(1)) == false)
            {
                throw new TestException("Cannot parse command-line arguments");
            }

            if (!Directory.Exists(options.Path))
            {
                throw new TestException($"Path does not exist: {options.Path}");
            }

            return options;
        }

        #endregion
    }
}