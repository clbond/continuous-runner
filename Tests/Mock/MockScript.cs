using System.ComponentModel.Composition;
using ContinuousRunner.Impl;

using Moq;

namespace ContinuousRunner.Tests.Mock
{
    public class MockScript : IMockScript
    {
        [Import] private readonly IMockFile _mockFile;

        public IScript Get(string content)
        {
            var parser = new JavaScriptParser();

            var script = new Mock<IScript>();
            script.SetupGet(s => s.File).Returns(_mockFile.FromString("js", content));
            script.SetupGet(s => s.ExpressionTree).Returns(parser.Parse(content));
            script.Name = content;

            return script.Object;
        }
    }
}
