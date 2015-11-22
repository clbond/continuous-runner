using System.IO;

namespace TestRunner
{
    public class ScriptLoader : IScriptLoader
    {
        private readonly IModuleReader _moduleReader;

        private readonly IScriptParser _scriptParser;

        public ScriptLoader(IModuleReader moduleReader, IScriptParser scriptParser)
        {
            _moduleReader = moduleReader;

            _scriptParser = scriptParser;
        }

        #region Implementation of IScriptLoader

        public IScript Load(FileInfo script)
        {
            return new Script(_moduleReader, _scriptParser)
            {
                File = script,
                SyntaxTree = _scriptParser.Parse(script)
            };
        }

        #endregion
    }
}

