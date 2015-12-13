using ContinuousRunner.Impl;

using Moq;

namespace ContinuousRunner.Tests.Mock
{
    public class MockScript
    {
        public static IScript Get(string content)
        {
            var parser = new Parser();

            var script = new Mock<IScript>();

            script.SetupGet(s => s.File).Returns(MockFile.FromString(content));
            script.SetupGet(s => s.ExpressionTree).Returns(parser.Parse(content));

            return script.Object;
        }
    }
}
