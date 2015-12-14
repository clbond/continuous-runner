using System.ComponentModel.Composition;

using Microsoft.ClearScript;

namespace ContinuousRunner.Frameworks.RequireJs
{
    public class FrameworkImpl : IFramework
    {
        [Import] private readonly IReferenceResolver _referenceResolver;

        public Framework Framework => Framework.RequireJs;

        public void Install(IProjectSource source, ScriptEngine engine)
        {
            if (source is IScript == false)
            {
                return;
            }

            var script = (IScript) source;

            var shimImpl = new ModuleDefinitions(_referenceResolver, engine, script);

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
    }
}
