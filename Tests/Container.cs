using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Autofac;

using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace ContinuousRunner.Tests
{
    public static class Container
    {
        public static IContainer CreateContainer(Action<ContainerBuilder> build = null)
        {
            ConfigureLogging();

            var builder = new ContainerBuilder();

            builder.RegisterModule<ContinuousRunnerModule>();

            build?.Invoke(builder);

            return builder.Build();
        }

        [SuppressMessage("Usage", "CC0022:Should dispose object", Justification = "NLog needs to dispose of its targets"
            )]
        private static void ConfigureLogging()
        {
            LogManager.EnableLogging();

            var layout = Layout.FromString(
                @"${shortdate}|${level:uppercase=true}|${logger}|${message} ${exception:format=tostring}");

            var targets = new List<Target>
                          {
                              new OutputDebugStringTarget
                              {
                                  Layout = layout,
                                  Name = "Debug Log"
                              },
                              new ConsoleTarget
                              {
                                  Layout = layout,
                                  Name = "Console"
                              }
                          };

            LogManager.Configuration = new LoggingConfiguration();

            foreach (var target in targets)
            {
                LogManager.Configuration.AddTarget(target);
                LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, target));
            }

            LogManager.ReconfigExistingLoggers();
        }
    }
}
