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
                    console.log('loading: ' + names + ' ' + names[0] + ' ' + names[1]);
                    var loaded = names.length > 0
                        ? requireImpl.RequireMultiple(names)
                        : requireImpl.RequireSingle(names);
  
                    if (typeof callback === 'function') {
                      return callback.apply(null, loaded);
                    }

                    return callack;
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
 
                  if (typeof body === 'function') {
                    body = body.apply(null, require(deps));
                  }

                  requireImpl.DefineModule(name, deps || [], body);
                };

                define['amd'] = true;");
        }
    }
}
