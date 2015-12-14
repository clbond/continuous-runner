using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ContinuousRunner.Frameworks.Language
{
    public class Detect : IDetector<Framework>
    {
        #region Implementation of IDetector<Framework>

        public Framework Analyze(IScript script)
        {
            if (script.File != null)
            {
                Func<string[], string, bool> compare =
                    (possible, ext) => possible.Any(t => string.Equals(ext, t, StringComparison.InvariantCultureIgnoreCase));

                if (compare(Constants.FileExtensions.TypeScript, script.File.Extension))
                {
                    return Framework.TypeScript;
                }

                if (compare(Constants.FileExtensions.CoffeeScript, script.File.Extension))
                {
                    return Framework.CoffeeScript;
                }

                if (compare(Constants.FileExtensions.JavaScript, script.File.Extension))
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
