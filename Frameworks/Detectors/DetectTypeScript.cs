using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ContinuousRunner.Frameworks.Detectors
{
    public class DetectTypeScript : IDetector
    {
        #region Implementation of IDetector

        public Framework Detect(IScript script)
        {
            if (script.File != null)
            {
                Func<string, bool> ts =
                    ext =>
                    Constants.FileExtensions.TypeScript.Any(
                        t => string.Equals(ext, t, StringComparison.InvariantCultureIgnoreCase));

                if (ts(script.File.Extension))
                {
                    return Framework.TypeScript;
                }

                Func<string, bool> js =
                    ext =>
                    Constants.FileExtensions.JavaScript.Any(
                        t => string.Equals(ext, t, StringComparison.InvariantCultureIgnoreCase));

                if (js(script.File.Extension))
                {
                    return Framework.JavaScript;
                }
            }

            if (string.IsNullOrEmpty(script.Content))
            {
                return Framework.None;
            }

            var referenceExp = new Regex("^///(\\s*)?<reference",
                                         RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

            if (referenceExp.IsMatch(script.Content))
            {
                return Framework.TypeScript;
            }

            var amdExp = new Regex("^///(\\s*)?<amd-dependency",
                                   RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

            if (amdExp.IsMatch(script.Content))
            {
                return Framework.TypeScript;
            }

            return Framework.None;
        }

        #endregion
    }
}
