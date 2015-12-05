using System.IO;

namespace ContinuousRunner.Tests
{
    public class TestInstance : IInstanceContext
    {
        #region Implementation of IInstanceContext

        public DirectoryInfo ScriptsRoot => new DirectoryInfo(Path.GetTempPath());

        public string ModuleNamespace => @"Test";

        #endregion
    }
}
