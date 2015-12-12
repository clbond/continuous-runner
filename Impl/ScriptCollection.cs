using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ContinuousRunner.Impl
{
    public class ScriptCollection : IScriptCollection
    {
        [Import] private readonly IInstanceContext _instanceContext;
        
        private readonly ISet<IScript> _collection = new HashSet<IScript>(new ScriptComparer()); 

        #region Implementation of IScriptCollection
        
        public IEnumerable<IScript> GetScripts(Func<FileInfo, IScript> loader)
        {
            Func<string, bool> isScript =
                ext => Constants.FileExtensions.TypeScript.Any(
                           t => string.Equals(ext, t, StringComparison.InvariantCultureIgnoreCase)) ||
                       Constants.FileExtensions.JavaScript.Any(
                           j => string.Equals(ext, j, StringComparison.InvariantCultureIgnoreCase));

            var files = _instanceContext.ScriptsRoot.GetFiles(string.Empty, SearchOption.AllDirectories).Where(f => isScript(f.Extension));

            return files.Select(loader).Where(script => script != null);
        }

        public IEnumerable<IScript> GetTestScripts(Func<FileInfo, IScript> loader)
        {
            var testExpr = new Regex("(spec|tests|test).js$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var files = _instanceContext.ScriptsRoot.GetFiles(string.Empty, SearchOption.AllDirectories).Where(f => testExpr.IsMatch(f.Name));
            
            return files.Select(loader).Where(script => script != null);
        }

        public IEnumerable<IScript> GetProductScripts(Func<FileInfo, IScript> loader)
        {
            return GetScripts(loader).Except(GetTestScripts(loader));
        }

        public void Add(IScript script)
        {
            _collection.Add(script);
        }

        public void Remove(IScript script)
        {
            _collection.Remove(script);
        }

        public IScript FindScript(Func<IScript, bool> matcher)
        {
            return _collection.SingleOrDefault(matcher);
        }

        public IScript FindTestScript(Func<IScript, bool> matcher)
        {
            throw new NotImplementedException();
        }
        
        #endregion
    }
}