using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using ContinuousRunner.Extensions;

using Jint.Parser.Ast;

namespace ContinuousRunner.Impl
{
    public class ModuleReader : IModuleReader
    {
        #region Private members

        [Import] private readonly IInstanceContext _context;
        
        [Import] private readonly IReferenceResolver _referenceResolver;

        [Import]
        private readonly Regex _fileExpression = new Regex(".(js|ts)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Implementation of IModuleReader

        public ModuleDefinition Get(IScript script, Func<string, IScript> referenceLoader)
        {
            var define = GetDefinitionExpression(script);

            var moduleName = GetModuleNameFromScript(script.File);

            var references = GetDirectReferences(moduleName, define, referenceLoader);

            return new ModuleDefinition
                   {
                       References = references,
                       Expression = GetDefinitionReturnExpression(define),
                       ModuleName = moduleName
                   };
        }

        private IList<IScript> GetDirectReferences(string fromModule, CallExpression callExpression, Func<string, IScript> referenceLoader)
        {
            var dependencies = GetDependencies(fromModule, callExpression);

            return dependencies.Select(referenceLoader).ToList();
        }

        public string GetModuleNameFromScript(FileInfo fileInfo)
        {
            var path = _context.ScriptsRoot.FullName;

            if (fileInfo.FullName.StartsWith(path))
            {
                return PathToModule(fileInfo.FullName.Substring(path.Length + 1));
            }

            throw new TestException($"Cannot determine module name from {fileInfo.FullName}");
        }

        #endregion

        #region Private methods

        private static CallExpression GetDefinitionExpression(IScript script)
        {
            return script.ExpressionTree?.Root?.SearchSingle<CallExpression>(IsDefineStatement);
        }

        private static bool IsDefineStatement(CallExpression functionExpr)
        {
            if (functionExpr.Callee.Type != SyntaxNodes.Identifier)
            {
                return false;
            }

            var identifier = functionExpr?.Callee.As<Identifier>();

            return identifier?.Name == "define";
        }

        private static ExpressionTree<SyntaxNode> GetDefinitionReturnExpression(CallExpression define)
        {
            var expr = define?.Arguments.LastOrDefault();
            if (expr != null)
            {
                return new ExpressionTree<SyntaxNode>(expr);
            }

            return null;
        }

        private IEnumerable<string> GetDependencies(string fromModule, CallExpression define)
        {
            var referencesExpr = define?.Arguments.FirstOrDefault(a => a.Type == SyntaxNodes.ArrayExpression);

            return ExtractReferencesFromArgument(fromModule, referencesExpr);
        }

        private IEnumerable<string> ExtractReferencesFromArgument(string fromModule, SyntaxNode expression)
        {
            if (expression == null)
            {
                return Enumerable.Empty<string>();
            }

            var array = expression.As<ArrayExpression>();

            return array.Elements
                        .Where(element => element.Type == SyntaxNodes.Literal)
                        .Select(element => GetAbsoluteReference(fromModule, element))
                        .Where(referencedScript => referencedScript != null);
        }

        private string GetAbsoluteReference(string sourceReference, SyntaxNode required)
        {
            return _referenceResolver.ResolveToModule(sourceReference, required.As<Literal>().Value as string);
        }

        private string PathToModule(string path)
        {
            path = _fileExpression.Replace(path, string.Empty);

            var split = new[] {Path.DirectorySeparatorChar};

            var segments = path.Split(split, StringSplitOptions.RemoveEmptyEntries).ToList();

            segments.Insert(0, _context.ModuleNamespace);

            return string.Join("/", segments);
        }

        #endregion
    }
}