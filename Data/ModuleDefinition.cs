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
            return ModuleReferences.Select(@ref => _dependencyResolver(@ref)).Where(script => script != null);
        }
    }
}