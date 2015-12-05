using Autofac;

using ContinuousRunner.Frameworks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContinuousRunner.Tests
{
    [TestClass]
    public class FrameworkDetectorTests
    {
        [TestMethod]
        public void TestJasmineDetection()
        {
            using (var container = Container.CreateContainer())
            {
                var detector = container.Resolve<IFrameworkDetector>();

                const string jasmineScript =
                    @"describe('Test description', function () {
                    it('Test item', function () {});
                  });";

                var script = ScriptMock.Get(jasmineScript);

                var frameworks = detector.DetectFrameworks(script);

                Assert.AreEqual(Framework.Jasmine, frameworks);
            }
        }

        [TestMethod]
        public void TestRequireJsDetection()
        {
            using (var container = Container.CreateContainer())
            {
                var detector = container.Resolve<IFrameworkDetector>();

                const string requireScript = @"define();";

                var script = ScriptMock.Get(requireScript);

                var frameworks = detector.DetectFrameworks(script);

                Assert.AreEqual(Framework.RequireJs, frameworks);
            }
        }

        [TestMethod]
        public void TestNodeJsDetection()
        {
            using (var container = Container.CreateContainer())
            {
                var detector = container.Resolve<IFrameworkDetector>();

                const string nodeScript = @"var path = require('path');";

                var script = ScriptMock.Get(nodeScript);

                var frameworks = detector.DetectFrameworks(script);

                Assert.AreEqual(Framework.NodeJs, frameworks);
            }
        }

        [TestMethod]
        public void TestCombinedScriptDetection()
        {
            using (var container = Container.CreateContainer())
            {
                var detector = container.Resolve<IFrameworkDetector>();

                const string combinedScript =
                    @"var fs = require('fs');
                      define(function () {
                        describe('Test description', function () {
                          it('Test item', function () {});
                        });
                      });";

                var script = ScriptMock.Get(combinedScript);

                var frameworks = detector.DetectFrameworks(script);

                Assert.IsTrue(frameworks.HasFlag(Framework.Jasmine));
                Assert.IsTrue(frameworks.HasFlag(Framework.NodeJs));
                Assert.IsTrue(frameworks.HasFlag(Framework.RequireJs));
            }
        }
    }
}
