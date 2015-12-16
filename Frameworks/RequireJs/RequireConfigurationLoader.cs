using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

using Jint.Parser.Ast;

using Magnum.Extensions;

namespace ContinuousRunner.Frameworks.RequireJs
{
    using Extensions;

    public class RequireConfigurationLoader : IRequireConfigurationLoader
    {
        [Import] private readonly ICachedScripts _cachedScripts;

        [Import] private readonly IRequireConfigurationParser _configurationParser;

        [Import] private readonly ILoader<IScript> _loader;
        
        #region Implementation of IConfigurationLoader

        public IRequireConfiguration Load(IEnumerable<FileInfo> search)
        {
            var candidates = search.SelectMany(Load).Where(config => config != null).ToArray();
            if (candidates.Any())
            {
                return Merge(candidates);
            }

            return null;
        }

        public IEnumerable<IRequireConfiguration> Load(FileInfo fileInfo)
        {
            var script = _cachedScripts.Get(fileInfo, f => _loader.Load(f));
            if (script?.ExpressionTree?.Root == null)
            {
                return Enumerable.Empty<IRequireConfiguration>();
            }

            var match = script.ExpressionTree.Root.Search<CallExpression>(IsRequireConfigCall).ToArray();
            if (match.Any())
            {
                return match.Select(m => TryParse(script.ExpressionTree.Root, m.Arguments.FirstOrDefault()));
            }

            return Enumerable.Empty<IRequireConfiguration>();
        }

        public bool IsRequireConfigCall(CallExpression callExpression)
        {
            if (callExpression.Callee.Type != SyntaxNodes.MemberExpression)
            {
                return false;
            }

            var memberExpr = (MemberExpression)callExpression.Callee;

            return MatchIdentifier(memberExpr.Object, new[] { @"require", @"requirejs" })
                && MatchIdentifier(memberExpr.Property, new[] { @"config", @"configure" });
        }

        #endregion

        #region Private methods

        private IRequireConfiguration TryParse(SyntaxNode root, Expression expression)
        {
            try
            {
                return Parse(root, expression);
            }
            catch
            {
                return null;
            }
        }

        private IRequireConfiguration Parse(SyntaxNode root, Expression expression)
        {
            if (expression == null)
            {
                return null;
            }

            switch (expression.Type)
            {
                case SyntaxNodes.Identifier:
                    var identifier = (Identifier) expression;

                    var declaration =
                        root.Search<VariableDeclaration>(expr => expr.Declarations.Any(d => d.Id.Name == identifier.Name))
                            .SelectMany(expr => expr.Declarations)
                            .SingleOrDefault(d => d.Id.Name == identifier.Name);

                    if (declaration != null)
                    {
                        return ExtractFromVariableDeclaration(root, declaration);
                    }

                    var declarator = root.Search<VariableDeclarator>(expr => expr.Id.Name == identifier.Name).SingleOrDefault();
                    if (declarator != null)
                    {
                        return ExtractFromVariableDeclaration(root, declarator);
                    }

                    throw new ApplicationException($"Cannot find matching variable declarator: {identifier.Name}");

                case SyntaxNodes.ObjectExpression:
                    return ParseFromObjectExpression(root, (ObjectExpression) expression);

                default:
                    throw new ArgumentOutOfRangeException(nameof(expression.Type), "Cannot parse require.config() argument");
            }
        }

        private IRequireConfiguration ParseFromObjectExpression(SyntaxNode root, ObjectExpression expression)
        {
            return _configurationParser.Parse(expression, root);
        }

        private IRequireConfiguration ExtractFromVariableDeclaration(SyntaxNode root, VariableDeclarator declarator)
        {
            if (declarator.Init.Type == SyntaxNodes.ObjectExpression)
            {
                return ParseFromObjectExpression(root, (ObjectExpression) declarator.Init);
            }

            return null;
        }

        private static IRequireConfiguration Merge(IEnumerable<IRequireConfiguration> candidates)
        {
            var config = new RequireConfiguration();

            foreach (var candidate in candidates)
            {
                foreach (var url in candidate.BaseUrl)
                {
                    config.BaseUrl.Add(url);
                }

                candidate.Maps.Each(kvp => config.Maps.Add(kvp));
                candidate.Packages.Each(p => config.Packages.Add(p));
                candidate.Paths.Each(p => config.Paths.Add(p));
            }

            return config;
        }

        private static bool MatchIdentifier(Expression expression, IEnumerable<string> matches)
        {
            if (expression.Type != SyntaxNodes.Identifier)
            {
                return false;
            }

            var identifier = (Identifier) expression;

            return matches.Any(m => string.Equals(m, identifier.Name, StringComparison.InvariantCulture));
        }

        #endregion
    }
}
