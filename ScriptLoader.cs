using System.IO;

namespace TestRunner
{
    public class ScriptLoader : IScriptLoader
    {
        #region IScriptLoader

        public IScript Load(FileInfo script)
        {
            return new Script
            {
                File = script,
                SyntaxTree = Parse(script)
            };
        }

        #endregion

        #region Private methods

        private static SyntaxTree Parse(FileInfo script)
        {
            var parser = new Jint.Parser.JavaScriptParser();

            return new SyntaxTree
            {
                Root = parser.Parse(GetScript(script))
            };
        }

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

