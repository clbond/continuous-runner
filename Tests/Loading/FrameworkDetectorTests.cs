using Autofac;
using ContinuousRunner.Frameworks;
using ContinuousRunner.Tests.Mock;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace ContinuousRunner.Tests.Loading
{
    public class FrameworkDetectorTests : BaseTest
    {
        public FrameworkDetectorTests(ITestOutputHelper helper)
            : base(helper)
        {}

        [Fact]
        public void TestJasmineDetection()
        {
            using (var container = CreateContainer())
            {
                var detector = container.Resolve<IFrameworkDetector>();

                const string jasmineScript =
                    @"describe('Test description', function () {
                    it('Test item', function () {});
                  });";

                var script = MockScript.Get(jasmineScript);

                var frameworks = detector.DetectFrameworks(script);

                frameworks.Should().Be(Framework.Jasmine);
            }
        }

        [Fact]
        public void TestRequireJsDetection()
        {
            using (var container = CreateContainer())
            {
                var detector = container.Resolve<IFrameworkDetector>();

                const string requireScript = @"define();";

                var script = MockScript.Get(requireScript);

                var frameworks = detector.DetectFrameworks(script);

                frameworks.Should().Be(Framework.RequireJs);
            }
        }

        [Fact]
        public void TestNodeJsDetection()
        {
            using (var container = CreateContainer())
            {
                var detector = container.Resolve<IFrameworkDetector>();

                const string nodeScript = @"var path = require('path');";

                var script = MockScript.Get(nodeScript);

                var frameworks = detector.DetectFrameworks(script);

                frameworks.Should().Be(Framework.NodeJs);
            }
        }

        [Fact]
        public void TestCombinedScriptDetection()
        {
            using (var container = CreateContainer())
            {
                var detector = container.Resolve<IFrameworkDetector>();

                const string combinedScript =
                    @"var fs = require('fs');
                      define(function () {
                        describe('Test description', function () {
                          it('Test item', function () {});
                        });
                      });";

                var script = MockScript.Get(combinedScript);

                var frameworks = detector.DetectFrameworks(script);

                frameworks.Should().HaveFlag(Framework.Jasmine);
                frameworks.Should().HaveFlag(Framework.NodeJs);
                frameworks.Should().HaveFlag(Framework.RequireJs);
            }
        }
    }
}
