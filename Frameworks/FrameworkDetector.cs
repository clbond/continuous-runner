using System.Collections.Generic;
using System.Linq;

using NLog;

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
            var result = _detectors.Aggregate(Framework.None, (f, detector) => f | detector.Detect(script));

            return ResolveConflicts(result);
        }

        public Framework DetectFrameworks(IEnumerable<IScript> scripts)
        {
            var result = scripts.Aggregate(Framework.None, (current, script) => current | DetectFrameworks(script));

            return ResolveConflicts(result);
        }

        #endregion

        #region Private methods

        private static Framework ResolveConflicts(Framework result)
        {
            var logger = LogManager.GetCurrentClassLogger();

            // If we detected conflicting frameworks, then unset them both because our detector isn't working on this code
            if (result.HasFlag(Framework.NodeJs) &&
                result.HasFlag(Framework.RequireJs))
            {
                result &= ~Framework.NodeJs;
                result &= ~Framework.RequireJs;

                logger.Error("Detected conflicting frameworks (RequireJS and NodeJS); unsetting both");
            }

            if (result.HasFlag(Framework.JavaScript) &&
                result.HasFlag(Framework.TypeScript))
            {
                result &= ~Framework.JavaScript;

                logger.Error("Detected conflicting languages (both TypeScript and JavaScript); unsetting JavaScript");
            }

            return result;
        }

        #endregion
    }
}
