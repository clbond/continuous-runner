using ContinuousRunner.Frameworks;
using ContinuousRunner.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class FrameworkDetectorTests
    {
        [TestMethod]
        public void TestJasmineDetection()
        {
            var detector = new FrameworkDetector();

            const string jasmineScript =
                @"describe('Test description', function () {
                    it('Test item', function () {});
                  });";

            var script = ScriptMock.Get(jasmineScript);

            var frameworks = detector.DetectFrameworks(script);

            Assert.Equals(frameworks, Framework.Jasmine);
        }

        [TestMethod]
        public void TestRequireJsDetection()
        {
            var detector = new FrameworkDetector();

            const string requireScript = @"define();";

            var script = ScriptMock.Get(requireScript);

            var frameworks = detector.DetectFrameworks(script);

            Assert.Equals(frameworks, Framework.RequireJs);
        }

        [TestMethod]
        public void TestNodeJsDetection()
        {
            var detector = new FrameworkDetector();

            const string nodeScript = @"module.exports = {};";

            var script = ScriptMock.Get(nodeScript);

            var frameworks = detector.DetectFrameworks(script);

            Assert.Equals(frameworks, Framework.NodeJs);
        }

        [TestMethod]
        public void TestCombinedScriptDetection()
        {
            var detector = new FrameworkDetector();

            const string combinedScript =
                @"define(function () {
                    describe('Test description', function () {
                        it('Test item', function () {});
                    });
                  });

                  module.exports = {};";

            var script = ScriptMock.Get(combinedScript);

            var frameworks = detector.DetectFrameworks(script);

            Assert.IsTrue(frameworks.HasFlag(Framework.Jasmine));
            Assert.IsTrue(frameworks.HasFlag(Framework.NodeJs));
            Assert.IsTrue(frameworks.HasFlag(Framework.RequireJs));
        }
    }
}
