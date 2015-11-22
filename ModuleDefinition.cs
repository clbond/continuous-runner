using System.Collections.Generic;

namespace TestRunner
{
    public class ModuleDefinition
    {
        public string ModuleName { get; set; }

        public IList<IScript> Dependencies { get; set; }

        public Jint.Parser.Ast.Expression Expression { get; set; }
    }
}
