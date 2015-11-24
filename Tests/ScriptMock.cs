using ContinuousRunner;
using ContinuousRunner.Impl;
using Moq;

namespace Tests
{
    public static class ScriptMock
    {
        public static IScript Get(string content)
        {
            var parser = new Parser();

            var script = new Mock<IScript>();

            script.SetupGet(s => s.File).Returns(FileMock.FromString(content));
            script.SetupGet(s => s.SyntaxTree).Returns(parser.Parse(content));

            return script.Object;
        }
    }
}
