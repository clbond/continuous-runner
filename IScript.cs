using Jint.Parser.Ast;

namespace ContinuousRunner
{
    using Data;

    public interface IScript : IProjectSource
    {
        /// <summary>
        /// An abstract syntax tree of a parsed JavaScript file
        /// </summary>
        ExpressionTree<SyntaxNode> ExpressionTree { get; }

        /// <summary>
        /// The module definition extracted from the JavaScript code (define() statement details)
        /// </summary>
        ModuleDefinition Module { get; }        
    }
}