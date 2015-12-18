using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

using Autofac;

using Jint.Parser.Ast;

using Magnum.Extensions;

namespace ContinuousRunner.Frameworks.RequireJs
{
    using Extensions;

    public class RequireConfigurationLoader : IRequireConfigurationLoader, ISubscription<SourceChangedEvent>
    {
        [Import] private readonly IComponentContext _componentContext;

        [Import] private readonly ICachedScripts _cachedScripts;

        [Import] private readonly IRequireConfigurationParser _configurationParser;

        private readonly ISet<string> _configurationFiles = new HashSet<string>();

        #region Implementation of IConfigurtationLoader

        public IRequireConfiguration Load(IEnumerable<FileInfo> search)
        {
            var config = new RequireConfiguration();

            var candidates = search.SelectMany(Load).Where(c => c != null).ToArray();

            foreach (var candidate in candidates)
            {
                MergeWith(config, candidate);
            }

            return config;
        }

        public IEnumerable<IRequireConfiguration> Load(FileInfo fileInfo)
        {
            var script = _cachedScripts.Load(fileInfo);

            if (script?.ExpressionTree?.Root == null)
            {
                return Enumerable.Empty<IRequireConfiguration>();
            }

            var match = script.ExpressionTree.Root.Search<CallExpression>(IsRequireConfigCall).ToArray();
            if (match.Any())
            {
                if (_configurationFiles.Contains(fileInfo.FullName))
                {
                    _configurationFiles.Add(fileInfo.Name); // watch for changes to this file
                }

                return match.Select(m => TryParse(script.ExpressionTree.Root, m.Arguments.FirstOrDefault()));
            }

            return Enumerable.Empty<IRequireConfiguration>();
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

        private static bool MatchIdentifier(Expression expression, IEnumerable<string> matches)
        {
            if (expression.Type != SyntaxNodes.Identifier)
            {
                return false;
            }

            var identifier = (Identifier) expression;

            return matches.Any(m => string.Equals(m, identifier.Name, StringComparison.InvariantCulture));
        }

        private static bool IsRequireConfigCall(CallExpression callExpression)
        {
            if (callExpression.Callee.Type != SyntaxNodes.MemberExpression)
            {
                return false;
            }

            var memberExpr = (MemberExpression)callExpression.Callee;

            return MatchIdentifier(memberExpr.Object, new[] { @"require", @"requirejs" })
                && MatchIdentifier(memberExpr.Property, new[] { @"config", @"configure" });
        }

        private void MergeWithExisting(IEnumerable<IRequireConfiguration> candidates)
        {
            var existing = _componentContext.Resolve<IRequireConfiguration>();

            foreach (var candidate in candidates)
            {
                MergeWith(existing, candidate);
            }
        }

        private static void MergeWith(IRequireConfiguration existing, IRequireConfiguration candidate)
        {
            foreach (var url in candidate.BaseUrl)
            {
                if (existing.BaseUrl.Contains(url) == false)
                {
                    existing.BaseUrl.Add(url);
                }
            }

            existing.Maps.Each(kvp => candidate.Maps.Add(kvp));
            existing.Packages.Each(p => candidate.Packages.Add(p));
            existing.Paths.Each(p => candidate.Paths.Add(p));
        }

        #endregion

        #region Implementation of ISubscription<in SourceChangedEvent>

        public void Handle(SourceChangedEvent @event)
        {
            if (@event.SourceFile is IScript == false)
            {
                return;
            }

            var script = (IScript) @event.SourceFile;
            
            if (_configurationFiles.Contains(@event.SourceFile.File.FullName) || script.ExpressionTree.Root.Search<CallExpression>(IsRequireConfigCall).Any())
            {
                var config = Load(script.File);
                if (config != null)
                {
                    MergeWithExisting(config);
                }
            }
        }

        #endregion
    }
}
