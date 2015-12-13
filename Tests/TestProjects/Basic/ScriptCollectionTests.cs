using System;
using System.IO;
using System.Linq;
using Autofac;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

using ContinuousRunner.Tests.Mock;

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
            var root = MockFile.TestFile<DirectoryInfo>(nameof(TestProjects), nameof(Basic), "Scripts");

            using (var container = CreateTypicalContainer(root))
            {
                var collection = container.Resolve<IScriptCollection>();

                var loader = container.Resolve<IScriptLoader>();
                
                Func<FileInfo, IScript> load = fi => loader.Load(fi);

                var all = collection.GetScripts(load).ToArray();
                all.Count().Should().Be(3);

                var f1 = all.SingleOrDefault(f => f.Module.ModuleName == "Tests/File1");
                f1.Should().NotBeNull();

                var f1refs = f1.Module.References.ToArray();
                f1refs.Length.Should().Be(1);

                var refToF2 = f1refs[0];
                refToF2.Module.ModuleName.Should().Be("Tests/File2");

                var tests = collection.GetTestScripts(load).ToArray();
                tests.Count().Should().Be(1);
            }
        }
    }
}
