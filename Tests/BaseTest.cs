using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Autofac;
using Magnum.Extensions;
using Moq;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using Xunit.Abstractions;

namespace ContinuousRunner.Tests
{
    public class BaseTest : IDisposable
    {
        private readonly ITestOutputHelper _helper;

        private readonly MemoryTarget _memoryTarget;

        protected BaseTest(ITestOutputHelper helper)
        {
            _helper = helper;

            _memoryTarget = new MemoryTarget
                            {
                                Name = "xUnit log",
                                Layout = GetStandardLayout()
                            };

            if (LogManager.Configuration == null)
            {
                LogManager.Configuration = new LoggingConfiguration();
            }

            LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, _memoryTarget));            

            LogManager.EnableLogging();
        }

        protected static IContainer CreateContainer(Action<ContainerBuilder> build = null)
        {
            ConfigureLogging();

            var builder = new ContainerBuilder();

            builder.RegisterModule<ContinuousRunnerModule>();

            build?.Invoke(builder);

            return builder.Build();
        }

        protected static IContainer CreateTypicalContainer(DirectoryInfo root,
            Action<ContainerBuilder> additionalBuild = null)
        {
            var instanceContext = new Mock<IInstanceContext>();
            instanceContext.SetupGet(i => i.ScriptsRoot).Returns(root);
            instanceContext.SetupGet(i => i.ModuleNamespace).Returns(nameof(Tests));

            Action<ContainerBuilder> build =
                cb =>
                {
                    cb.Register(c => instanceContext.Object).As<IInstanceContext>();

                    additionalBuild?.Invoke(cb);
                };

            return CreateContainer(build);
        }

        [SuppressMessage("Usage", "CC0022:Should dispose object", Justification = "NLog oddness")]
        protected static void ConfigureLogging()
        {
            LogManager.EnableLogging();

            var layout = GetStandardLayout();

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

            if (LogManager.Configuration == null)
            {
                LogManager.Configuration = new LoggingConfiguration();
            }

            foreach (var target in targets)
            {
                LogManager.Configuration.AddTarget(target.Name, target);
                LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, target));
            }

            LogManager.ReconfigExistingLoggers();
        }

        #region Implementation of IDisposable

        public virtual void Dispose()
        {
            LogManager.Flush();

            _memoryTarget.Logs.Each(l => _helper.WriteLine(l));
        }

        #endregion

        #region Private methods

        private static Layout GetStandardLayout()
        {
            return Layout.FromString(
                @"${shortdate}|${level:uppercase=true}|${logger}|${message} ${exception:format=tostring}");
        }

        #endregion
    }
}