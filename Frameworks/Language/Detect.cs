using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ContinuousRunner.Frameworks.Language
{
    public class Detect : IDetector<Framework>
    {
        #region Implementation of IDetector<Framework>

        public Framework Analyze(IProjectSource source)
        {
            if (source.File != null)
            {
                Func<string[], string, bool> compare =
                    (possible, ext) => possible.Any(t => string.Equals(ext, t, StringComparison.InvariantCultureIgnoreCase));

                if (compare(Constants.FileExtensions.TypeScript, source.File.Extension))
                {
                    return Framework.TypeScript;
                }

                if (compare(Constants.FileExtensions.CoffeeScript, source.File.Extension) ||
                    compare(Constants.FileExtensions.CSharp, source.File.Extension))
                {
                    return GetLanguageFromCoffeeScriptOrCSharp(source);
                }

                if (compare(Constants.FileExtensions.JavaScript, source.File.Extension))
                {
                    return Framework.JavaScript;
                }
            }

            if (string.IsNullOrEmpty(source.Content))
            {
                return Framework.None;
            }

            var referenceExp = new Regex("^///(\\s*)?<reference",
                                         RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

            if (referenceExp.IsMatch(source.Content))
            {
                return Framework.TypeScript;
            }

            var amdExp = new Regex("^///(\\s*)?<amd-dependency",
                                   RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

            if (amdExp.IsMatch(source.Content))
            {
                return Framework.TypeScript;
            }

            return Framework.None;
        }

        /// <summary>
        /// Attempt a best guess at whether a file contains C# or CoffeeScript code (both languages share the same file extension)
        /// </summary>
        private static Framework GetLanguageFromCoffeeScriptOrCSharp(IProjectSource source)
        {
            if (string.IsNullOrEmpty(source.Content))
            {
                return Framework.None;
            }

            using (var sr = new StringReader(source.Content))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    if (line.StartsWith("#"))
                    {
                        return Framework.CoffeeScript;
                    }

                    if (line.StartsWith("//") ||
                        line.StartsWith("using") ||
                        line.StartsWith("namespace") ||
                        line.StartsWith("class"))
                    {
                        return Framework.CSharp;
                    }

                    return Framework.CoffeeScript;
                }
            }

            return Framework.None;
        }

        #endregion
    }
}
