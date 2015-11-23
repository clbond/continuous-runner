using System;
using Autofac;
using NLog;

namespace ContinuousRunner.Console
{
    public class Application
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            try
            {
                var options = CommandLineOptions.FromArgs(args);

                using (var container = Container.Build(options))
                {
                    var loader = container.Resolve<IScriptLoader>();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Bootstrap of continuous runner failed: {ex.Message}");
            }
        }
    }
}
