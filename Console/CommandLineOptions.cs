using System;
using System.IO;

using CommandLine;

namespace ContinuousRunner.Console
{
    public class CommandLineOptions : IInstanceContext
    {
        #region Command-line arguments and options

        [Option('r', "solution-path",
            Required = true,
            HelpText = "The path to the solution or project root")]
        public string SolutionPath { get; set; }

        [Option('s', "scripts-path",
            Required = true,
            HelpText = "The path to search for JavaScript modules and tests")]
        public string ScriptsPath { get; set; }

        #endregion
        
        #region Implementation of IInstanceContext

        public DirectoryInfo SolutionRoot => new DirectoryInfo(SolutionPath);

        public DirectoryInfo ScriptsRoot => new DirectoryInfo(ScriptsPath);

        [Option('m', "module-namespace",
            DefaultValue = "R2CIQ",
            Required = false,
            HelpText = "The module namespace that our source files reside in")]
        public string ModuleNamespace { get; set; }

        #endregion

        #region Factory methods

        public static IInstanceContext FromArgs(string[] args)
        {
            var options = new CommandLineOptions();

            if (Parser.Default.ParseArgumentsStrict(args, options, () => Environment.Exit(1)) == false)
            {
                throw new TestException("Cannot parse command-line arguments");
            }

            if (!Directory.Exists(options.ScriptsPath))
            {
                throw new TestException($"Path does not exist: {options.ScriptsPath}");
            }

            return options;
        }

        #endregion
    }
}