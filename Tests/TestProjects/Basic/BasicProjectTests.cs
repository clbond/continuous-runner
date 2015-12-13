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
            instanceContext.SetupGet(i => i.ModuleNamespace).Returns("Test");

            Action<ContainerBuilder> build = cb => cb.Register(c => instanceContext.Object).As<IInstanceContext>();

            using (var container = Container.CreateContainer(build))
            {
                var collection = container.Resolve<IScriptCollection>();

                var loader = container.Resolve<IScriptLoader>();

                var cached = container.Resolve<ICachedScripts>();

                Func<FileInfo, IScript> load = fi => loader.TryLoad(fi);

                var all = collection.GetScripts(load).ToArray();

                var tests = collection.GetTestScripts(load).ToArray();
            }
        }
    }
}
