using System;
using System.IO;
using System.Linq;
using Autofac;
using ContinuousRunner.Frameworks.RequireJs;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

using ContinuousRunner.Tests.Mock;
using NLog;

// ReSharper disable PossibleNullReferenceException

namespace ContinuousRunner.Tests.TestProjects.Basic
{
    public class ScriptCollectionTests : BaseTest
    {
        public ScriptCollectionTests(ITestOutputHelper helper)
            : base(helper)
        {
        }

        private void RunInContext(Action<IComponentContext, IScriptCollection> f)
        {
            var root = MockFile.TempFile<DirectoryInfo>(nameof(TestProjects), nameof(Basic), "Scripts");

            using (var container = CreateTypicalContainer(root))
            {
                var collection = container.Resolve<IScriptCollection>();

                f(container, collection);
            }
        }

        private void RunInContextAndLoad(Action<IComponentContext, IScript[]> f)
        {
            RunInContext(
                (container, collection) =>
                {
                    var scripts = container.Resolve<IScriptCollection>().GetScripts().ToArray();

                    f(container, scripts);
                });
        }

        [Fact]
        public void TestLoadScripts()
        {
            RunInContextAndLoad(
                (container, scripts) =>
                {
                    scripts.Length.Should().Be(5);
                });
        }

        [Fact]
        public void TestModuleReferences()
        {
            var logger = LogManager.GetCurrentClassLogger();

            RunInContextAndLoad(
                (container, scripts) =>
                {
                    var script = scripts.SingleOrDefault(f => f.Module.ModuleName == "Tests/File1");
                    script.Should().NotBeNull();

                    var scriptRefs = script.Module.References.ToArray();
                    scriptRefs.Length.Should().Be(1);

                    var file2Ref = scriptRefs[0];
                    file2Ref.Module.ModuleName.Should().Be("Tests/File2");

                    var scriptCollection = container.Resolve<IScriptCollection>();

                    var tests = scriptCollection.GetTestScripts().ToArray();

                    var testWriter = container.Resolve<ITestWriter>();

                    foreach (var t in tests)
                    {
                        logger.Info($"Suites and tests in {t}:");

                        testWriter.Write(t.Suites);
                    }

                    tests.Count().Should().Be(1);
                });
        }

        [Fact]
        public void TestRequireConfiguration()
        {
            RunInContextAndLoad(
                (container, scripts) =>
                {
                    var configLoader = container.Resolve<IRequireConfigurationLoader>();

                    var config = configLoader.Load(scripts.Select(s => s.File));

                    config.Should().NotBeNull();

                    config.BaseUrl.Count.Should().Be(1);
                });
        }
    }
}