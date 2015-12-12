using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContinuousRunner.Tests
{
    [TestClass]
    public class ScriptRunnerTests
    {
        [TestMethod]
        public void TestBasicTestDefinition()
        {
            using (var container = Container.CreateContainer())
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
