using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

using Jint.Parser.Ast;

namespace ContinuousRunner.Frameworks.RequireJs
{
    using Extensions;

    public class ConfigurationLoader : IConfigurationLoader
    {
        [Import] private readonly IParser<SyntaxNode> _parser;

        [Import] private readonly IConfigurationParser _configurationParser;

        private IRequireConfiguration _cached;

        #region Implementation of IConfigurationLoader

        public IRequireConfiguration Load(IEnumerable<FileInfo> search)
        {
            if (_cached == null)
            {
                _cached = search.Select(Load).FirstOrDefault(config => config != null);
            }

            return _cached;
        }

        public IRequireConfiguration Load(FileInfo script)
        {
            var parsed = _parser.TryParse(script);

            var match = parsed?.Root.Search<CallExpression>(IsRequireConfigCall).FirstOrDefault();
            if (match != null)
            {
                return Parse(parsed.Root, match.Arguments.FirstOrDefault());
            }

            return null;
        }

        #endregion

        #region Private methods

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
                        return ExtractFromVariableDeclaration(declaration);
                    }

                    var declarator = root.Search<VariableDeclarator>(expr => expr.Id.Name == identifier.Name).SingleOrDefault();
                    if (declarator != null)
                    {
                        return ExtractFromVariableDeclaration(declarator);
                    }

                    throw new ApplicationException($"Cannot find matching variable declarator: {identifier.Name}");

                case SyntaxNodes.ObjectExpression:
                    return ParseFromObjectExpression((ObjectExpression) expression);

                default:
                    throw new ArgumentOutOfRangeException(nameof(expression.Type), "Cannot parse require.config() argument");
            }
        }

        private IRequireConfiguration ParseFromObjectExpression(ObjectExpression expression)
        {
            return _configurationParser.Parse(expression);
        }

        private IRequireConfiguration ExtractFromVariableDeclaration(VariableDeclarator declarator)
        {
            if (declarator.Init.Type == SyntaxNodes.ObjectExpression)
            {
                return ParseFromObjectExpression((ObjectExpression) declarator.Init);
            }

            return null;
        }

        private static bool IsRequireConfigCall(CallExpression callExpression)
        {
            if (callExpression.Callee is MemberExpression == false)
            {
                return false;
            }

            var memberExpr = (MemberExpression) callExpression.Callee;

            return MatchIdentifier(memberExpr.Object, new[] {@"require", @"requirejs"}) && MatchIdentifier(memberExpr.Property, new[] {@"config", @"configure"});
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
