using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ContinuousRunner.Tests
{
    [TestClass]
    public class ScriptTests
    {
        [TestMethod]
        public void TestBasicTestDefinition()
        {
            var instanceContext = new Mock<IInstanceContext>();
            instanceContext.SetupGet(i => i.ScriptsRoot).Returns(new DirectoryInfo(Path.GetTempPath()));
            instanceContext.SetupGet(i => i.ModuleNamespace).Returns(string.Empty);

            Action<ContainerBuilder> build = cb => cb.Register(c => instanceContext.Object).As<IInstanceContext>();

            using (var container = Container.CreateContainer(build))
            {
                const string content = @"describe('Foo', function () { it('Bar', function () { debugger; }); });";

                var loader = container.Resolve<IScriptLoader>();

                var script = loader.Load(content);
                Assert.IsNotNull(script);
                Assert.IsNotNull(script.ExpressionTree);

                var runner = container.Resolve<IScriptRunner>();

                var result = runner.RunAsync(script).ToArray();

                var success = Task.WaitAll(result.Cast<Task>().ToArray(), Constants.TestTimeout);
                if (success == false)
                {
                    throw new TestException("Timed out while waiting for script execution result");
                }
            }
        }
    }
}
