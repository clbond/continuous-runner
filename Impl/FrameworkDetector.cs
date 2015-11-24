using System.Collections.Generic;
using ContinuousRunner.Frameworks;

namespace ContinuousRunner.Impl
{
    public class FrameworkDetector : IFrameworkDetector
    {
        #region Implementation of IFrameworkDetector

        public Framework DetectFrameworks(IScript script)
        {
            throw new System.NotImplementedException();
        }

        public Framework DetectFrameworks(IEnumerable<IScript> script)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
