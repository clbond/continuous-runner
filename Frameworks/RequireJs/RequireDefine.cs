using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Microsoft.ClearScript;
using Microsoft.JScript;

namespace ContinuousRunner.Frameworks.RequireJs
{
    public class RequireDefine
    {
        [Import] private readonly IReferenceResolver _referenceResolver;

        [Import] private readonly IRequireConfiguration _requireConfiguration;

        private readonly Dictionary<string, Func<object>> _defines = new Dictionary<string, Func<object>>();

        private readonly ScriptEngine _scriptEngine;

        private readonly IScript _fromScript;

        public RequireDefine(ScriptEngine scriptEngine, IScript fromScript)
        {
            _scriptEngine = scriptEngine;

            _fromScript = fromScript;
        }

        #region Methods exposed to the JavaScript runtime

        public object RequireMultiple(ArrayList modules)
        {
            return Require(_fromScript.Module.ModuleName, modules.Cast<string>());
        }

        public object RequireSingle(string modules)
        {
            return Require(_fromScript.Module.ModuleName, modules);
        }

        public void DefineModule(string name, ArrayList dependencies, Func<ArrayList, object> body)
        {
            var deps = dependencies.Cast<string>().ToArray();

            Define(_fromScript.Module.ModuleName, name, deps, body);
        }

        #endregion

        #region Private methods

        private void Register(string modulePath, string moduleName, Func<object> load)
        {
            if (string.IsNullOrEmpty(moduleName))
            {
                throw new InvalidOperationException("Cannot register a module with no name");
            }

            if (load == null)
            {
                throw new ArgumentNullException(nameof(load));
            }

            _defines[ToAbsolutePath(modulePath, moduleName)] = load;
        }

        private string ToAbsolutePath(string modulePath, string moduleName)
        {
            return _referenceResolver.Resolve(modulePath, moduleName);
        }

        private void Define(string fromModule, string moduleName, string[] dependencies, Func<ArrayList, object> definition)
        {
            if (dependencies == null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            if (string.IsNullOrEmpty(moduleName))
            {
                moduleName = fromModule;
            }

            Require(moduleName, dependencies);

            Register(fromModule, moduleName, () => EvaluateDefine(LoadRequires(moduleName, dependencies), definition));
        }
        
        private static object EvaluateDefine(ICollection<object> dependencies, Func<ArrayList, dynamic> definition)
        {
            var list = new ArrayList((ICollection) dependencies);
            
            return definition?.Invoke(list);
        }

        private object[] LoadRequires(string moduleName, string[] dependencies)
        {
            return Require(moduleName, dependencies);
        }

        private object Require(string fromModule, string dependency)
        {
            var p = ToAbsolutePath(fromModule, dependency);

            if (_defines.ContainsKey(p) == false)
            {
                var local = _referenceResolver.ModuleReferenceToFile(p);
                if (local != null)
                {
                    Register(fromModule, p, () => LoadScript(local));
                }
            }

            if (_defines.ContainsKey(p) == false)
            {
                return null;
            }

            return _defines[p]();
        }

        private object[] Require(string fromModule, IEnumerable<string> dependencies)
        {
            return (from dependency in (string[]) dependencies select Require(fromModule, dependency)).ToArray();
        }

        private object LoadScript(FileInfo fileInfo)
        {
            using (var stream = fileInfo.OpenRead())
            {
                using (var sr = new StreamReader(stream))
                {
                    var content = sr.ReadToEnd();

                    return _scriptEngine.Evaluate(content);
                }
            }
        }

        #endregion
    }
}