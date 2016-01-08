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

        [Import] private readonly ICachedScripts _loader;
        
        private readonly ISet<IScript> _collection = new HashSet<IScript>(new ScriptComparer()); 

        #region Implementation of IScriptCollection

        public IEnumerable<FileInfo> GetScriptFiles()
        { 
            Func<string[], string, bool> match =
                (extensions, fileExtension) =>
                extensions.Any(e => string.Equals(e, fileExtension, StringComparison.InvariantCultureIgnoreCase));

            Func<string, bool> isScript =
                ext => /*match(Constants.FileExtensions.TypeScript, ext) ||*/
                       match(Constants.FileExtensions.JavaScript, ext);

            var root = _instanceContext.ScriptsRoot;

            if (root == null || root.Exists == false)
            {
                throw new InvalidOperationException($"Sripts root is not set or does not exist: {root}");
            }

            var files = root.GetFiles(@"*", SearchOption.AllDirectories);
            
            return files.Where(f => isScript(f.Extension));
        }

        public IEnumerable<IScript> GetScripts()
        {
            return GetScriptFiles().Select(_loader.Load).Where(script => script != null);
        }

        public IEnumerable<IScript> GetTestScripts()
        {
            var testExpr = new Regex("(spec|tests|test).js$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var files = GetScriptFiles().Where(f => testExpr.IsMatch(f.Name));
            
            return files.Select(_loader.Load).Where(script => script != null);
        }

        public IEnumerable<IScript> GetProductScripts()
        {
            return GetScripts().Except(GetTestScripts());
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