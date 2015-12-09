using System;
using System.IO;
using System.Linq;

namespace ContinuousRunner.Impl
{
    using Data;

    public class Parser : IParser
    {
        #region Implementation of IScriptParser

        public virtual ExpressionTree Parse(FileInfo fileInfo)
        {
            return Parse(GetScript(fileInfo));
        }

        public virtual ExpressionTree Parse(string script)
        {
            if (script == null)
            {
                throw new ArgumentNullException(nameof(script));
            }

            var parser = new Jint.Parser.JavaScriptParser();

            try
            {
                return new ExpressionTree(parser.Parse(script));
            }
            catch (Exception ex)
            {
                var first = script.Take(256);

                throw new TestException($"Failed to parse JavaScript content ({first}...)", ex);
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
