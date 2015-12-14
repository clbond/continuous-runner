using System.IO;

namespace ContinuousRunner.Impl
{
    public class ClassLoader : ILoader<IClass>
    {
        #region Implementation of ILoader<IClass>

        public IClass Load(FileInfo script)
        {
            throw new System.NotImplementedException();
        }

        public IClass TryLoad(FileInfo script)
        {
            throw new System.NotImplementedException();
        }

        public IClass Load(string content)
        {
            throw new System.NotImplementedException();
        }

        public IClass LoadModule(IClass fromScript, string relativeReference)
        {
            throw new System.NotImplementedException();
        }

        public IClass LoadModule(string absoluteReference)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
