using System.Collections.Generic;

namespace ContinuousRunner.Data
{
    public class ModuleDefinition
    {
        public string ModuleName { get; set; }
        
        public IList<IScript> References { get; set; }

        public ExpressionTree Expression { get; set; }
    }
}