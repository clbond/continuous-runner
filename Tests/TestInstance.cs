using System.IO;

namespace ContinuousRunner.Tests
{
    public class TestInstance : IInstanceContext
    {
        #region Implementation of IInstanceContext

        public DirectoryInfo ScriptsRoot
        {
            get { return new DirectoryInfo(Path.GetTempPath()); }
        }

        public string ModuleNamespace
        {
            get { return string.Empty; }
        }

        #endregion
    }
}
