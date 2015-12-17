using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Autofac;
using ContinuousRunner.Frameworks.RequireJs;
using ContinuousRunner.Tests.Mock;

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

        private MemoryTarget _memoryTarget;

        protected BaseTest(ITestOutputHelper helper)
        {
            _helper = helper;
        }

        private static IContainer CreateContainer(Action<ContainerBuilder> build = null)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<ContinuousRunnerModule>();

            builder.RegisterAssemblyTypes(typeof (BaseTest).Assembly)
                   .AsImplementedInterfaces()
                   .OnActivated(args => PropertyInjector.InjectProperties(args.Context, args.Instance));

            build?.Invoke(builder);

            return builder.Build();
        }

        protected IContainer CreateTypicalContainer(DirectoryInfo root, Action<ContainerBuilder> additionalBuild = null)
        {
            ConfigureLogging();

            IContainer container = null;

            var instanceContext = new Mock<IInstanceContext>();
            instanceContext.SetupGet(i => i.SolutionRoot).Returns(root);
            instanceContext.SetupGet(i => i.ScriptsRoot).Returns(root);
            instanceContext.SetupGet(i => i.ModuleNamespace).Returns(nameof(Tests));
            
            Action<ContainerBuilder> build =
                cb =>
                {
                    cb.Register(c => instanceContext.Object).As<IInstanceContext>();

                    additionalBuild?.Invoke(cb);
                };

            container = CreateContainer(build);

            return container;
        }
        
        protected IContainer CreateTypicalContainer(Action<ContainerBuilder> additionalBuild = null)
        {
            return CreateTypicalContainer(MockFile.TempFile<DirectoryInfo>(), additionalBuild);
        }

        [SuppressMessage("Usage", "CC0022:Should dispose object", Justification = "NLog oddness")]
        protected void ConfigureLogging()
        {
            var layout = GetStandardLayout();

            if (_memoryTarget == null)
            {
                _memoryTarget = new MemoryTarget
                                {
                                    Name = "Unit Test Log",
                                    Layout = layout
                                };
            }

            lock (typeof (LogManager))
            {
                if (LogManager.Configuration == null)
                {
                    LogManager.Configuration = new LoggingConfiguration();
                }

                var targets = new List<Target>
                              {
                                  _memoryTarget,
                                  new OutputDebugStringTarget {Name = "Debug", Layout = layout},
                                  new TraceTarget {Name = "Trace", Layout = layout}
                              };

                foreach (var target in targets)
                {
                    LogManager.Configuration.AddTarget(target.Name, target);
                    LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
                }

                LogManager.EnableLogging();

                LogManager.ReconfigExistingLoggers();
            }
        }

        #region Implementation of IDisposable

        void IDisposable.Dispose()
        {
            if (_memoryTarget != null)
            {
                LogManager.Flush();

                var logs = new List<string>(_memoryTarget.Logs); // must clone otherwise the statement below this one will throw a 'collection modified' exception

                logs.Each(l => _helper.WriteLine(l));

                _memoryTarget = null;
            }
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