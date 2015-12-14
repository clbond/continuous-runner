using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;

using Microsoft.ClearScript.V8;

namespace ContinuousRunner.Frameworks.RequireJs
{
    public class FrameworkImpl : IFramework
    {
        private readonly Dictionary<string, object> _defines = new Dictionary<string, object>();

        private readonly IInstanceContext _instanceContext;

        public FrameworkImpl(IInstanceContext instanceContext)
        {
            _instanceContext = instanceContext;
        }

        public Framework Framework => Framework.RequireJs;

        public void Install(V8ScriptEngine engine)
        {
            var root = _instanceContext.ScriptsRoot.FullName;

            Func<string[], object> require = req => Require(engine, root, req);

            Action<string, object, object> define = (module, deps, @object) => Define(engine, root, module, deps, @object);

            dynamic shimImpl = new {define, require};

            engine.AddHostObject(nameof(shimImpl), shimImpl);

            engine.Execute(
                @"var require = function (names, callback) {
                      var loaded = shimImpl.require(names);
                      if (callback) {
                        callback.apply(null, loaded);
                      }
                  };");

            engine.Execute(
              @"var define = function (name, deps, body) {
                  // Shift to handle anonymous definitions
                  if (typeof deps === 'function') {
                      body = deps;
                      deps = name;
                      name = '';
                  }
 
                  // 'resolve' the dependencies in place
                  deps = deps || [];
                  for (var i = 0; i < deps.length; i++)
                      deps[i] = require(deps[i]);

                  shimImpl.define(name, body.apply(null, deps));
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
            throw new NotImplementedException();
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

            foreach (var dependency in (string[])deps)
            {
                
            }

            Register(engine, fromModule, moduleName as string, v8 => obj);
        }

        public object Require(V8ScriptEngine engine, string fromModule, object dependencies)
        {
            var results = new List<object>();

            if (dependencies is string)
            {
                var p = ToAbsolutePath(fromModule, (string) dependencies);

                var local = GetLocalFile(p);
                if (local != null)
                {
                    Register(engine, fromModule, p, v8 => LoadScript(v8, local));

                    return _defines.ContainsKey(p) ? _defines[p] : null;
                }
                else
                {
                    return null;
                }
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
                    var local = GetLocalFile(ToAbsolutePath(fromModule, dependency));
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

        private object LoadScript(V8ScriptEngine v8, string file)
        {
            var fileInfo = new FileInfo(file);
            if (fileInfo.Exists == false)
            {
                throw new ArgumentException("File does not exist", nameof(file));
            }

            using (var stream = fileInfo.OpenRead())
            {
                using (var sr = new StreamReader(stream))
                {
                    var content = sr.ReadToEnd();
                    return v8.Evaluate(content);
                }
            }
        }

        private string GetLocalFile(string dependency)
        {
            throw new NotImplementedException();
        }
    }
}
