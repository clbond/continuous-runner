using System.Collections.Generic;
using System.Linq;

namespace ContinuousRunner.Frameworks
{
    public class FrameworkDetector : IFrameworkDetector
    {
        private readonly IEnumerable<IDetector> _detectors;

        public FrameworkDetector(IEnumerable<IDetector> detectors)
        {
            _detectors = detectors;
        }

        #region Implementation of IFrameworkDetector

        public Framework DetectFrameworks(IScript script)
        {
            return _detectors.Aggregate(Framework.None, (f, detector) => f | detector.Detect(script));
        }

        public Framework DetectFrameworks(IEnumerable<IScript> scripts)
        {
            return scripts.Aggregate(Framework.None,
                (current, script) => current | _detectors.Aggregate(Framework.None, (f, detector) => f | detector.Detect(script)));
        }

        #endregion
    }
}
