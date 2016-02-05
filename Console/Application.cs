using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Autofac;
using ContinuousRunner.Work;
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

                RunAsync(options).Wait();
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Bootstrap of continuous runner failed: {ex.Message}");
            }
        }

        private static async Task RunAsync(IInstanceContext options)
        {
            var logger = LogManager.GetCurrentClassLogger();

            using (var consoleReader = new ConsoleReader())
            using (var container = Container.Build(options))
            {
                consoleReader.Start();

                LoadScriptsAsync(container);

                var watcher = container.Resolve<IWatcher>();

                using (var watchHandle = watcher.Watch())
                {
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
                                await RunTestsAsync(container).ConfigureAwait(false);
                                break;
                        }
                    }
                }
            }
        }

        private static async Task RunTestsAsync(IComponentContext componentContext)
        {
            var collection = componentContext.Resolve<IScriptCollection>();

            var executor = componentContext.Resolve<IConcurrentExecutor>();

            var tasks =
                collection.GetTestScripts()
                          .Select(
                              ts =>
                              executor.ExecuteAsync(
                                  componentContext.Resolve<ExecuteScriptWork>(new TypedParameter(typeof (IScript), ts))))
                          .ToList();

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private static async void LoadScriptsAsync(IComponentContext componentContext)
        {
            var logger = LogManager.GetCurrentClassLogger();

            var collection = componentContext.Resolve<IScriptCollection>();

            var queue = componentContext.Resolve<IConcurrentExecutor>();

            logger.Info("Loading scripts");

            var tasks = new List<Task<IExecutionResult>>();

            foreach (var script in collection.GetScripts())
            {
                var description = $"Loaded: {script.File.Name}";

                logger.Info(description);

                var t = queue.ExecuteAsync(new ExecuteScriptWork(componentContext.Resolve<IRunner<IScript>>(), script, description));

                tasks.Add(t);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}