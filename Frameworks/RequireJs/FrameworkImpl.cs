using System.ComponentModel.Composition;
using Autofac;
using Microsoft.ClearScript;

namespace ContinuousRunner.Frameworks.RequireJs
{
    public class FrameworkImpl : IFramework
    {
        [Import] private readonly IComponentContext _componentContext;

        public Framework Framework => Framework.RequireJs;

        public void Install(IProjectSource source, ScriptEngine engine)
        {
            if (source is IScript == false)
            {
                return;
            }

            var script = (IScript) source;

            var requireImpl = _componentContext.Resolve<RequireDefine>(
                new TypedParameter(typeof (ScriptEngine), engine),
                new TypedParameter(typeof (IScript), script));

            engine.AddHostObject(nameof(requireImpl), requireImpl);

            engine.Execute(
                @"var require = function (names, callback) {
                    var loaded = requireImpl.require(names);
  
                    if (typeof callback === 'function') {
                      callback.apply(null, loaded);
                    }

                    return loaded;
                  };");
            
            engine.Execute(
              @"var define = function (name, deps, body) {
                  if (typeof name !== 'string') {
                      body = deps;
                      deps = name;
                      name = null;
                  }

                  if (deps instanceof Array === false) {
                    if (body != null) {
                      throw new Error('define() was called with incomprehensible arguments');
                    }

                    body = deps;

                    deps = [];
                  }
 
                  // 'resolve' the dependencies in place
                  deps = deps || [];
                  for (var i = 0; i < deps.length; i++) {
                    deps[i] = require(deps[i]);
                  }

                  requireImpl.define(name, deps, body);

                  if (typeof body === 'function') {
                    body.apply(null, deps);
                  }
                };

                define['amd'] = true;");
        }
    }
}
