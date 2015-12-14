using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Microsoft.ClearScript.V8;

using NLog;

namespace ContinuousRunner.Frameworks
{
    public class FrameworkDetector : IFrameworkDetector
    {
        [Import] private readonly IEnumerable<IDetector<Framework>> _detectors;

        [Import] private readonly IEnumerable<IFramework> _frameworks;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region Implementation of IFrameworkDetector

        public Framework DetectFrameworks(IProjectSource script)
        {
            var result = _detectors.Aggregate(Framework.None, (f, detector) => f | detector.Analyze(script));

            var resolved = ResolveConflicts(result);

            _logger.Debug($"Framework detect: {script}: {resolved.ToString()}");

            return resolved;
        }

        public Framework DetectFrameworks(IEnumerable<IProjectSource> scripts)
        {
            return scripts.Select(DetectFrameworks).Aggregate(Framework.None, (current, resolved) => current | resolved);
        }

        public void InstallFrameworks(IProjectSource source, Framework framework, V8ScriptEngine engine)
        {
            foreach (var impl in _frameworks.Where(impl => framework.HasFlag(impl.Framework)))
            {
                impl.Install(source, engine);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// A crappy way to detect a situation where we have analyed single script and come up with conflicting answers.
        /// </summary>
        /// <param name="result">The product of <seealso cref="DetectFrameworks(IScript)"/></param>
        private static Framework ResolveConflicts(Framework result)
        {
            var logger = LogManager.GetCurrentClassLogger();

            var resultClosure = result;

            Func<Framework[], bool> has = f => f.All(flag => resultClosure.HasFlag(flag));

            if (has(new[] {Framework.JavaScript, Framework.TypeScript}))
            {
                result &= ~Framework.JavaScript;

                logger.Error(
                    "Detected conflicting languages (both TypeScript and JavaScript); assuming TypeScript because it can encompass both");
            }

            if (has(new[] {Framework.JavaScript, Framework.CoffeeScript}))
            {
                result &= ~Framework.JavaScript;

                logger.Error("Detected conflicting frameworks (JavaScript and CoffeeScript); unsetting JavaScript");
            }

            if (has(new[] {Framework.CoffeeScript, Framework.TypeScript}))
            {
                result &= ~Framework.TypeScript;

                logger.Error("Detected conflicting frameworks (JavaScript and CoffeeScript); unsetting TypeScript");
            }

            return result;
        }

        #endregion
    }
}