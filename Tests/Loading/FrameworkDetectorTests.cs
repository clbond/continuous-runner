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
            using (var container = CreateTypicalContainer())
            {
                var detector = container.Resolve<IFrameworkDetector>();

                const string jasmineScript =
                    @"describe('Test description', function () {
                        it('Test item', function () {
                          console.log('Executed test');
                        });
                      });";

                var script = container.Resolve<IMockScript>().Get(jasmineScript);

                var frameworks = detector.DetectFrameworks(script);

                frameworks.Should().Be(Framework.Jasmine | Framework.JavaScript);
            }
        }

        [Fact]
        public void TestRequireJsDetection()
        {
            using (var container = CreateTypicalContainer())
            {
                var detector = container.Resolve<IFrameworkDetector>();

                const string requireScript = @"define();";

                var script = container.Resolve<IMockScript>().Get(requireScript);

                var frameworks = detector.DetectFrameworks(script);

                frameworks.Should().Be(Framework.RequireJs | Framework.JavaScript);
            }
        }

        [Fact]
        public void TestNodeJsDetection()
        {
            using (var container = CreateTypicalContainer())
            {
                var detector = container.Resolve<IFrameworkDetector>();

                const string nodeScript = @"var path = require('path');";

                var script = container.Resolve<IMockScript>().Get(nodeScript);

                var frameworks = detector.DetectFrameworks(script);

                frameworks.Should().Be(Framework.NodeJs | Framework.JavaScript);
            }
        }

        [Fact]
        public void TestCombinedScriptDetection()
        {
            using (var container = CreateTypicalContainer())
            {
                var detector = container.Resolve<IFrameworkDetector>();

                const string combinedScript =
                    @"var fs = require('fs');
                      define(function () {
                        describe('Test description', function () {
                          it('Test item', function () {});
                        });
                      });";

                var script = container.Resolve<IMockScript>().Get(combinedScript);

                var frameworks = detector.DetectFrameworks(script);

                frameworks.Should().HaveFlag(Framework.JavaScript);
                frameworks.Should().HaveFlag(Framework.Jasmine);
                frameworks.Should().HaveFlag(Framework.NodeJs);
                frameworks.Should().HaveFlag(Framework.RequireJs);
            }
        }
    }
}
