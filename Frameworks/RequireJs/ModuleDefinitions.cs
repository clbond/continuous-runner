using System;
using System.Collections.Generic;
using System.IO;

using JetBrains.Annotations;

using Microsoft.ClearScript;

namespace ContinuousRunner.Frameworks.RequireJs
{
    public class ModuleDefinitions
    {
        private readonly Dictionary<string, object> _defines = new Dictionary<string, object>();

        private readonly IReferenceResolver _referenceResolver;

        private readonly ScriptEngine _scriptEngine;

        private readonly IScript _fromScript;

        public ModuleDefinitions(
            [NotNull] IReferenceResolver referenceResolver,
            [NotNull] ScriptEngine scriptEngine,
            [NotNull] IScript fromScript)
        {
            _referenceResolver = referenceResolver;

            _scriptEngine = scriptEngine;

            _fromScript = fromScript;
        }
        
        #region Methods exposed to the JavaScript runtime

        public object require(object modules, Action callback = null)
        {
            return Require(_scriptEngine, _fromScript.Module.ModuleName, modules);
        }

        public void define(object name, object dependencies = null, object body = null)
        {
            Define(_scriptEngine, _fromScript.Module.ModuleName, name, dependencies, body);
        }

        #endregion

        #region Private methods

        public void Register(ScriptEngine engine, string modulePath, string moduleName, Func<ScriptEngine, object> load)
        {
            if (load == null)
            {
                throw new ArgumentNullException(nameof(load));
            }

            _defines[ToAbsolutePath(modulePath, moduleName)] = load(engine);
        }

        private string ToAbsolutePath(string modulePath, string moduleName)
        {
            return _referenceResolver.Resolve(modulePath, moduleName);
        }

        public void Define(ScriptEngine engine, string fromModule, object moduleName, object deps, object obj)
        {
            if (moduleName is Array)
            {
                obj = deps;
                deps = moduleName;
                moduleName = fromModule;
            }
            else if (moduleName is string)
            {
                if (!(deps is Array))
                {
                    obj = deps;
                    deps = null;
                }
            }
            else
            {
                obj = moduleName;
                deps = null;
                moduleName = fromModule;
            }

            if (deps == null)
            {
                deps = new string[0];
            }

            Require(engine, moduleName as string, deps);

            Register(engine, fromModule, moduleName as string, v8 => obj);
        }

        public object Require(ScriptEngine engine, string fromModule, object dependencies)
        {
            var results = new List<object>();

            var s = dependencies as string;
            if (s != null)
            {
                var p = ToAbsolutePath(fromModule, s);

                var local = _referenceResolver.ModuleReferenceToFile(p);
                if (local != null)
                {
                    Register(engine, fromModule, p, v8 => LoadScript(v8, local));

                    return _defines.ContainsKey(p) ? _defines[p] : null;
                }

                return null;
            }

            foreach (var dependency in (string[])dependencies)
            {
                object definition;
                if (_defines.TryGetValue(dependency, out definition))
                {
                    results.Add(definition);
                }
                else
                {
                    var local = _referenceResolver.ModuleReferenceToFile(ToAbsolutePath(fromModule, dependency));
                    if (local != null)
                    {
                        Register(engine, fromModule, dependency, v8 => LoadScript(v8, local));
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

        private static object LoadScript(ScriptEngine v8, FileInfo fileInfo)
        {
            using (var stream = fileInfo.OpenRead())
            {
                using (var sr = new StreamReader(stream))
                {
                    var content = sr.ReadToEnd();

                    return v8.Evaluate(content);
                }
            }
        }

        #endregion
    }
}
