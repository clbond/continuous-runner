using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Frameworks.RequireJs
{
    public class FrameworkImpl : IFramework
    {
        private readonly Dictionary<string, object> _defines = new Dictionary<string, object>();
        
        [Import] private readonly IReferenceResolver _referenceResolver;
        
        public Framework Framework => Framework.RequireJs;

        public void Install(IScript script, V8ScriptEngine engine)
        {
            var fromModule = script.Module.ModuleName;

            Func<object, object> require = req => Require(engine, fromModule, req);

            Action<object, object, object> define = (module, deps, @obj) => Define(engine, fromModule, module, deps, @obj);

            dynamic shimImpl = new {define, require};

            engine.AddHostObject(nameof(shimImpl), shimImpl);

            engine.Execute(
                @"var require = function (names, callback) {
                    var loaded = shimImpl.require(names);
  
                    if (typeof callback === 'function') {
                      callback.apply(null, loaded);
                    }

                    return loaded;
                  };");

            engine.Execute(
              @"var define = function (name, deps, body) {
                  // Shift to handle anonymous definitions
                  if (typeof name !== 'string') {
                      body = deps;
                      deps = name;
                      name = null;
                  }

                  if (typeof deps === 'function' && body == null) {
                    body = deps;
                    deps = [];
                  }
 
                  // 'resolve' the dependencies in place
                  deps = deps || [];
                  for (var i = 0; i < deps.length; i++)
                      deps[i] = require(deps[i]);

                  shimImpl.define(name, deps, body && body.toString());

                  if (typeof body === 'function') {
                    body.apply(null, deps);
                  }
                };

                define['amd'] = true;");
        }

        public void Register(V8ScriptEngine engine, string modulePath, string moduleName, Func<V8ScriptEngine, object> load)
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

        public void Define(V8ScriptEngine engine, string fromModule, object moduleName, object deps, object obj)
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

        public object Require(V8ScriptEngine engine, string fromModule, object dependencies)
        {
            var results = new List<object>();

            if (dependencies is string)
            {
                var p = ToAbsolutePath(fromModule, (string) dependencies);

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

        private static object LoadScript(V8ScriptEngine v8, FileInfo fileInfo)
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
    }
}
