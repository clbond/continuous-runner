using System;
using System.Collections.Generic;
using System.Linq;

namespace ContinuousRunner.Data
{
    public class ModuleDefinition
    {
        private readonly Func<string, IScript> _dependencyResolver;

        public ModuleDefinition(Func<string, IScript> dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public string ModuleName { get; set; }

        public IList<string> ModuleReferences { get; set; }

        public Jint.Parser.Ast.Expression Expression { get; set; }

        public IEnumerable<IScript> GetDependencies()
        {
            if (_dependencyResolver == null)
            {
                return Enumerable.Empty<IScript>();
            }

            return ModuleReferences.Select(_dependencyResolver).Where(script => script != null);
        }
    }
}