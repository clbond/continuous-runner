using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace ContinuousRunner.Frameworks
{
    public class FrameworkDetector : IFrameworkDetector
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IEnumerable<IDetector> _detectors;

        public FrameworkDetector(IEnumerable<IDetector> detectors)
        {
            _detectors = detectors;
        }

        #region Implementation of IFrameworkDetector

        public Framework DetectFrameworks(IScript script)
        {
            var result = _detectors.Aggregate(Framework.None, (f, detector) => f | ResolveConflicts(detector.Detect(script), script));

            return ResolveConflicts(result);
        }

        public Framework DetectFrameworks(IEnumerable<IScript> scripts)
        {
            var result = scripts.Aggregate(Framework.None, (current, script) => current | ResolveConflicts(DetectFrameworks(script), script));

            return ResolveConflicts(result);
        }

        #endregion

        #region Private methods

        private Framework ResolveConflicts(Framework result, IScript script = null)
        {
            // If we detected conflicting frameworks, then unset them both because our detector isn't working on this code
            if (result.HasFlag(Framework.NodeJs) &&
                result.HasFlag(Framework.RequireJs))
            {
                result &= ~Framework.NodeJs;
                result &= ~Framework.RequireJs;
            }

            var sb = new StringBuilder();
            if (script != null)
            {
                sb.Append(script.File.FullName);
                sb.Append(": ");
            }

            sb.Append("Detected conflicting frameworks (RequireJS and NodeJS); unsetting both");

            _logger.Error(sb.ToString());

            return result;
        }

        #endregion
    }
}
