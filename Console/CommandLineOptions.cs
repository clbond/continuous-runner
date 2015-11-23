using System;
using System.IO;
using CommandLine;

namespace ContinuousRunner.Console
{
    public class CommandLineOptions : IInstanceContext
    {
        #region Command-line arguments and options

        [Option('p', "path",
            Required = true,
            HelpText = "The path to search for JavaScript modules and tests")]
        public string Path { get; set; }

        #endregion
        
        #region Implementation of IInstanceContext

        public DirectoryInfo ScriptsRoot => new DirectoryInfo(Path);

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