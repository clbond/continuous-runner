namespace ContinuousRunner.Console
{
    using System;
    using Autofac;
    using NLog;

    public class Application
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            try
            {
                var options = CommandLineOptions.FromArgs(args);

                var containerBuilder = new ContainerBuilder();

                containerBuilder.RegisterInstance(options).As<IInstanceContext>();

                containerBuilder.RegisterModule<ContinuousRunner.Module>();

                var container = containerBuilder.Build();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Bootstrap of continuous runner failed: {ex.Message}");
            }
        }
    }
}
