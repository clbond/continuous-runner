using System;
using System.IO;
using System.Linq;

using Jint.Parser.Ast;

namespace ContinuousRunner.Impl
{
    public class JavaScriptParser : IParser<SyntaxNode>
    {
        #region Implementation of IScriptParser

        public virtual ExpressionTree<SyntaxNode> Parse(FileInfo fileInfo)
        {
            return Parse(GetScript(fileInfo));
        }

        public virtual ExpressionTree<SyntaxNode> Parse(string script)
        {
            if (script == null)
            {
                throw new ArgumentNullException(nameof(script));
            }

            var parser = new Jint.Parser.JavaScriptParser();

            try
            {
                return new ExpressionTree<SyntaxNode>(parser.Parse(script));
            }
            catch (Exception ex)
            {
                var first = string.Join(string.Empty, script.Take(512).ToArray());

                throw new TestException($"Failed to parse JavaScript content ({first}...): {ex}", ex);
            }
        }

        public ExpressionTree<SyntaxNode> TryParse(FileInfo fileInfo)
        {
            try
            {
                return Parse(fileInfo);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ExpressionTree<SyntaxNode> TryParse(string script)
        {
            try
            {
                return Parse(script);
            }
            catch (Exception)
            {
                return null;
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
