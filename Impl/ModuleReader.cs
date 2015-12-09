using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using ContinuousRunner.Extensions;

using JetBrains.Annotations;

using Jint.Parser.Ast;

using Magnum;

namespace ContinuousRunner.Impl
{
    using Data;

    public class ModuleReader : IModuleReader
    {
        #region Constructors

        public ModuleReader(
            [NotNull] IInstanceContext instanceContext,
            [NotNull] IReferenceTree referenceTree,
            [NotNull] IReferenceResolver referenceResolver)
        {
            Guard.AgainstNull(instanceContext, nameof(instanceContext));
            _context = instanceContext;

            Guard.AgainstNull(referenceTree, nameof(referenceTree));
            _referenceTree = referenceTree;

            Guard.AgainstNull(instanceContext, nameof(referenceResolver));
            _referenceResolver = referenceResolver;
        }

        #endregion

        #region Private members

        private readonly IInstanceContext _context;

        private readonly IReferenceTree _referenceTree;

        private readonly IReferenceResolver _referenceResolver;

        private readonly Regex _fileExpression = new Regex(".(js|ts)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Implementation of IModuleReader

        public ModuleDefinition Get(IScript script)
        {
            var define = GetDefinitionExpression(script);

            var references = GetDirectReferences(script, define);

            return new ModuleDefinition
                   {
                       References = references,
                       Expression = GetDefinitionReturnExpression(define),
                       ModuleName = GetModuleNameFromScript(script)
                   };
        }

        private IList<IScript> GetDirectReferences(IScript script, CallExpression callExpression)
        {
            var dependencies = GetDependencies(script, callExpression).ToArray();

            return new List<IScript>();
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

        private static ExpressionTree GetDefinitionReturnExpression(CallExpression define)
        {
            var expr = define?.Arguments.LastOrDefault();
            if (expr != null)
            {
                return new ExpressionTree(expr);
            }

            return null;
        }

        private IEnumerable<string> GetDependencies(IScript script, CallExpression define)
        {
            return ExtractReferencesFromArgument(script,
                                                 define?.Arguments.FirstOrDefault(
                                                     a => a.Type == SyntaxNodes.ArrayExpression));
        }

        private IEnumerable<string> ExtractReferencesFromArgument(IScript script, Expression expression)
        {
            if (expression == null)
            {
                return Enumerable.Empty<string>();
            }

            var array = expression.As<ArrayExpression>();

            return array.Elements
                        .Where(element => element.Type == SyntaxNodes.Literal)
                        .Select(element => GetAbsoluteReference(script, element))
                        .Where(referencedScript => referencedScript != null);
        }

        private string GetAbsoluteReference(IScript sourceReference, Expression required)
        {
            return _referenceResolver.Resolve(sourceReference, required.As<Literal>().Value as string);
        }

        private string GetModuleNameFromScript(IScript script)
        {
            if (script == null || script.File == null)
            {
                return null;
            }

            var path = _context.ScriptsRoot.FullName;

            if (script.File.FullName.StartsWith(path))
            {
                return PathToModule(script.File.FullName.Substring(path.Length + 1));
            }

            throw new TestException($"Cannot determine module name from {script.File.FullName}");
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