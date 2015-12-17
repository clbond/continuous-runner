using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

using Microsoft.ClearScript;

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

        public object[] RequireMultiple(string[] modules)
        {
                return Require(_fromScript.Module.ModuleName, modules);
        }

        public object RequireSingle(string modules)
        {
            return Require(_fromScript.Module.ModuleName, modules);
        }

        public void DefineModule(string name, string[] dependencies, dynamic body)
        {

            // Define(_fromScript.Module.ModuleName, name, dependencies, body);
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

        private void Define(string fromModule, string moduleName, string[] dependencies, object definition)
        {
            if (string.IsNullOrEmpty(moduleName))
            {
                moduleName = fromModule;
            }

            if (dependencies == null)
            {
                dependencies = new string[0];
            }

            Require(moduleName, dependencies);

            Register(fromModule, moduleName, () => EvaluateDefine(LoadRequires(moduleName, dependencies), definition));
        }

        private object EvaluateDefine(object[] dependencies, object definition)
        {
            throw new NotImplementedException();
        }

        private object[] LoadRequires(string moduleName, string[] dependencies)
        {
            return Require(moduleName, dependencies) as object[];
        }

        private object Require(string fromModule, string dependencies)
        {
            var p = ToAbsolutePath(fromModule, dependencies);

            if (_defines.ContainsKey(p) == false)
            {
                var local = _referenceResolver.ModuleReferenceToFile(p);
                if (local != null)
                {
                    Register(fromModule, p, () => LoadScript(local));
                }
            }

            return _defines[p]();
        }

        private object[] Require(string fromModule, string[] dependencies)
        {
            var results = new List<object>();

            foreach (var dependency in (string[])dependencies)
            {
                Func<object> definition;
                if (_defines.TryGetValue(dependency, out definition))
                {
                    results.Add(definition);
                }
                else
                {
                    var local = _referenceResolver.ModuleReferenceToFile(ToAbsolutePath(fromModule, dependency));
                    if (local != null)
                    {
                        Register(fromModule, dependency, () => LoadScript(local));
                        results.Add(local);
                    }
                    else
                    {
                        results.Add(null);
                    }
                }
            }

            return results.ToArray();
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
