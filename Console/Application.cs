using System;
using System.Linq;
using System.Threading.Tasks;

using Autofac;

using NLog;

namespace ContinuousRunner.Console
{
    public class Application
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();

            // ReSharper disable once CatchAllClause
            try
            {
                var options = CommandLineOptions.FromArgs(args);

                Run(options);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Bootstrap of continuous runner failed: {ex.Message}");
            }
        }

        private static void Run(IInstanceContext options)
        {
            var logger = LogManager.GetCurrentClassLogger();

            using (var consoleReader = new ConsoleReader())
            using (var container = Container.Build(options))
            {
                consoleReader.Start();

                LoadScripts(container);

                var watcher = container.Resolve<IWatcher>();

                var watchHandle = watcher.Watch();

                logger.Info("Entering run loop; press enter to stop; press R to force complete re-run");

                var stopping = false;

                while (!stopping)
                {
                    var key = consoleReader.Read(TimeSpan.FromSeconds(.5d));
                    if (key.HasValue == false)
                    {
                        continue;
                    }

                    switch (key.Value)
                    {
                        case '\r':
                        case '\n':
                            stopping = true;
                            break;
                        case 'R':
                        case 'r':
                            RunTests(container);
                            break;
                    }
                }

                watchHandle.Cancel();
            }
        }

        private static void RunTests(IComponentContext componentContext)
        {
            var logger = LogManager.GetCurrentClassLogger();

            logger.Debug("Running tests");

            var queue = componentContext.Resolve<IRunQueue>();

            var queued = queue.RunAsync().ToArray();
            if (queued.Any())
            {
                var results = Task.WhenAll(queued);

                results.Wait();

                foreach (var tr in results.Result)
                {
                    logger.Info(tr.ToString());
                }
            }
            else
            {
                logger.Info("No tests in queue");
            }
        }

        private static void LoadScripts(IComponentContext componentContext)
        {
            var logger = LogManager.GetCurrentClassLogger();

            var collection = componentContext.Resolve<IScriptCollection>();

            var queue = componentContext.Resolve<IRunQueue>();

            logger.Info("Loading scripts");

            var loader = componentContext.Resolve<ILoader<IScript>>();

            foreach (var script in collection.GetScripts(fi => loader.Load(fi)))
            {
                logger.Info("Loaded: {0}", script.File.Name);

                queue.Push(script);
            }
        }
    }
}