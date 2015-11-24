using System;
using System.IO;

namespace ContinuousRunner.Impl
{
    using Data;

    public class Parser : IParser
    {
        #region Implementation of IScriptParser

        public virtual SyntaxTree Parse(FileInfo fileInfo)
        {
            return Parse(GetScript(fileInfo));
        }

        public virtual SyntaxTree Parse(string script)
        {
            var parser = new Jint.Parser.JavaScriptParser();

            try
            {
                return new SyntaxTree
                {
                    Root = parser.Parse(script)
                };
            }
            catch (Exception ex)
            {
                throw new TestException($"Failed to parse JavaScript file: {fileInfo}", ex);
            }
        }

        #endregion

        #region Private methods

        private static string GetScript(FileInfo script)
        {
            using (var stream = script.OpenRead())
            {
                using (var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        #endregion
    }
}
