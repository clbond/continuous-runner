using System.Collections.Generic;

namespace ContinuousRunner.Data
{
    public class ModuleDefinition
    {
        public string ModuleName { get; set; }
        
        public IEnumerable<IScript> References { get; set; }

        public ExpressionTree Expression { get; set; }
    }
}