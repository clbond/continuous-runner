using System.Collections.Generic;

using Jint.Parser.Ast;

namespace ContinuousRunner
{
    public class ModuleDefinition
    {
        public string ModuleName { get; set; }
        
        public IEnumerable<IScript> References { get; set; }

        public ExpressionTree<SyntaxNode> Expression { get; set; }
    }
}