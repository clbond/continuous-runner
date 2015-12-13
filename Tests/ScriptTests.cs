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
            instanceContext.SetupGet(i => i.ModuleNamespace).Returns(nameof(Tests));

            Action<ContainerBuilder> build = cb => cb.Register(c => instanceContext.Object).As<IInstanceContext>();

            using (var container = Container.CreateContainer(build))
            {
                const string content =
                    @"define([], function () {
                        describe('Foo', function () {
                          it('Bar', function () {
                            debugger;
                          });
                        });
                      });";

                var fileInfo = FileMock.FromString(content);

                var loader = container.Resolve<IScriptLoader>();

                var script = loader.Load(fileInfo);
                Assert.IsNotNull(script);
                Assert.IsNotNull(script.ExpressionTree);

                Assert.IsNotNull(script.Module);

                var module = script.Module;

                Assert.AreEqual($"{nameof(Tests)}/{Path.GetFileNameWithoutExtension(fileInfo.Name)}", module.ModuleName);

            }
        }
    }
}
