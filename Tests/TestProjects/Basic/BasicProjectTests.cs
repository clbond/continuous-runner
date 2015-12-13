using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ContinuousRunner.Tests.TestProjects.Basic
{
    [TestClass]
    public class BasicProjectTests
    {
        [TestMethod]
        public void TestLoadScripts()
        {
            var root = FileMock.TestFile<DirectoryInfo>(nameof(TestProjects), nameof(Basic), "Scripts");

            var instanceContext = new Mock<IInstanceContext>();
            instanceContext.SetupGet(i => i.ScriptsRoot).Returns(root);
            instanceContext.SetupGet(i => i.ModuleNamespace).Returns("Tests");

            Action<ContainerBuilder> build = cb => cb.Register(c => instanceContext.Object).As<IInstanceContext>();

            using (var container = Container.CreateContainer(build))
            {
                var collection = container.Resolve<IScriptCollection>();

                var loader = container.Resolve<IScriptLoader>();

                var cached = container.Resolve<ICachedScripts>();

                Func<FileInfo, IScript> load = fi => loader.TryLoad(fi);

                var all = collection.GetScripts(load).ToArray();
                Assert.AreEqual(3, all.Count());

                var f1 = all.SingleOrDefault(f => f.Module.ModuleName == "Tests/File1");
                Assert.IsNotNull(f1);

                var f1refs = f1.Module.References.ToArray();
                Assert.AreEqual(1, f1refs.Length);

                var refToF2 = f1refs[0];
                Assert.AreEqual("Tests/File2", refToF2.Module.ModuleName);

                var tests = collection.GetTestScripts(load).ToArray();
                Assert.AreEqual(1, tests.Count());
            }
        }
    }
}
