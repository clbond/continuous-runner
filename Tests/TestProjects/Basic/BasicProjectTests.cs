using System;
using System.IO;
using System.Linq;
using System.Threading;
using Autofac;
using ContinuousRunner.Frameworks.RequireJs;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

using ContinuousRunner.Tests.Mock;

// ReSharper disable PossibleNullReferenceException

namespace ContinuousRunner.Tests.TestProjects.Basic
{
    public class BasicProjectTests : BaseTest
    {
        public BasicProjectTests(ITestOutputHelper helper)
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

                    var executor = container.Resolve<IConcurrentExecutor>();

                    var iteration = 0;

                    while (executor.PendingWork)
                    {
                        if (iteration++ > 30)
                        {
                            throw new TestException("Timed out waiting for test runs to complete");
                        }

                        Thread.Sleep(TimeSpan.FromMilliseconds(150d));
                    }
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

                    var scriptLoader = container.Resolve<ICachedScripts>();

                    var config = configLoader.Load(scripts.Select(s => s.File), f => scriptLoader.Load(f));

                    config.Should().NotBeNull();

                    config.BaseUrl.Count.Should().Be(1);
                });
        }

        [Fact]
        public void TestResolvedRequireConfiguration()
        {
            RunInContextAndLoad(
                (container, scripts) =>
                {
                    var configuration = container.Resolve<IRequireConfiguration>();

                    configuration.Should().NotBeNull();

                    configuration.BaseUrl.Count.Should().Be(1);
                    configuration.BaseUrl.First().Should().Be(".");
                });
        }

        [Fact]
        public void TestRequireConfigPaths()
        {
            RunInContext(
                (container, collection) =>
                {
                    var resolver = container.Resolve<IReferenceResolver>();

                    var f2 = resolver.Resolve("Tests/File1", "./File2");
                    f2.Should().NotBeNull();

                    var loader = container.Resolve<ICachedScripts>();

                    var loaded = loader.Load(f2);
                    loaded.Should().NotBeNull();

                    loaded.File.Name.Should().Be("File2.js");

                    var thirdPartyLib = resolver.Resolve("Tests/File1", "foo");
                    thirdPartyLib.Should().NotBeNull();
                    thirdPartyLib.Name.Should().Be("foo.js");
                    thirdPartyLib.Exists.Should().BeTrue();
                });
        }
    }
}