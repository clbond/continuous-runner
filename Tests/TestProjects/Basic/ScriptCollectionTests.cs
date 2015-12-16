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

namespace ContinuousRunner.Tests.TestProjects.Basic
{
    public class ScriptCollectionTests : BaseTest
    {
        public ScriptCollectionTests(ITestOutputHelper helper)
            : base(helper)
        {}

        [Fact]
        public void TestLoadScripts()
        {
            var root = MockFile.TempFile<DirectoryInfo>(nameof(TestProjects), nameof(Basic), "Scripts");

            using (var container = CreateTypicalContainer(root))
            {
                var collection = container.Resolve<IScriptCollection>();

                var loader = container.Resolve<ILoader<IScript>>();
                
                Func<FileInfo, IScript> load = fi => loader.Load(fi);

                var all = collection.GetScripts(load).ToArray();
                all.Count().Should().Be(5);
            }
        }

        [Fact]
        public void TestModuleReferences()
        {
            var root = MockFile.TempFile<DirectoryInfo>(nameof(TestProjects), nameof(Basic), "Scripts");

            using (var container = CreateTypicalContainer(root))
            {
                var logger = LogManager.GetCurrentClassLogger();

                var loader = container.Resolve<ILoader<IScript>>();

                Func<FileInfo, IScript> load = fi => loader.Load(fi);

                var collection = container.Resolve<IScriptCollection>();

                var all = collection.GetScripts(load).ToArray();

                var configLoader = container.Resolve<IConfigurationLoader>();

                var config = configLoader.Load(all.Select(s => s.File));
                config.Should().NotBeNull();
                config.BaseUrl.Count.Should().Be(1);

                var f1 = all.SingleOrDefault(f => f.Module.ModuleName == "Tests/File1");
                f1.Should().NotBeNull();

                var f1refs = f1.Module.References.ToArray();
                f1refs.Length.Should().Be(1);

                var refToF2 = f1refs[0];
                refToF2.Module.ModuleName.Should().Be("Tests/File2");

                var tests = collection.GetTestScripts(load).ToArray();

                var testWriter = container.Resolve<ITestWriter>();

                foreach (var t in tests)
                {
                    logger.Info($"Suites and tests in {t}:");

                    testWriter.Write(t.Suites);
                }

                tests.Count().Should().Be(1);

            }
        }
    }
}
