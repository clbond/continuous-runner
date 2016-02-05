using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

using Microsoft.ClearScript;

namespace ContinuousRunner.Frameworks.RequireJs
{
    public class RequireDefine
    {
        [Import] private readonly IReferenceResolver _referenceResolver;

        [Import] private readonly IPackageSystem _packageSystem;

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

            _packageSystem.Define(modulePath, moduleName, load);
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

        private dynamic[] LoadRequires(string moduleName, string[] dependencies)
        {
            return Require(moduleName, dependencies);
        }

        private dynamic Require(string fromModule, string moduleName)
        {
            var definition = _packageSystem.GetDefinition(fromModule, moduleName);
            if (definition == null)
            {
                LoadScript(_referenceResolver.Resolve(fromModule, moduleName));

                definition = _packageSystem.GetDefinition(fromModule, moduleName);
            }

            if (definition != null)
            {
                return definition();
            }

            return null;
        }

        private dynamic[] Require(string fromModule, IEnumerable<string> dependencies)
        {
            return (from dependency in dependencies select Require(fromModule, dependency)).ToArray();
        }

        private object LoadScript(FileInfo fileInfo)
        {
            if (fileInfo == null)
            {
                return null;
            }

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