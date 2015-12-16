using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Autofac;
using ContinuousRunner.Frameworks.RequireJs;
using ContinuousRunner.Impl;
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

        private IContainer CreateContainer(Action<ContainerBuilder> build = null)
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

            // ReSharper disable once AccessToModifiedClosure
            instanceContext.SetupGet(i => i.RequireConfig).Returns(() => GetRequireConfig(container));

            Action<ContainerBuilder> build =
                cb =>
                {
                    cb.Register(c => instanceContext.Object).As<IInstanceContext>();

                    additionalBuild?.Invoke(cb);
                };

            container = CreateContainer(build);

            return container;
        }

        protected static IRequireConfiguration GetRequireConfig(IComponentContext componentContext)
        {
            var loader = componentContext.Resolve<IConfigurationLoader>();

            var scriptLoader = componentContext.Resolve<ILoader<IScript>>();

            var collection = componentContext.Resolve<IScriptCollection>();

            var config = loader.Load(collection.GetScripts(f => scriptLoader.Load(f)).Select(s => s.File));

            return config;
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

                _memoryTarget.Logs.Each(l => _helper.WriteLine(l));
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