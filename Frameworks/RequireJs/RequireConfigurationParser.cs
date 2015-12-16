using System.Collections.Generic;
using System.Linq;
using Jint.Parser.Ast;

namespace ContinuousRunner.Frameworks.RequireJs
{
    using Extensions;

    public class RequireConfigurationParser : IRequireConfigurationParser
    {
        #region Implementation of IConfigurationParser

        public IRequireConfiguration Parse(ObjectExpression expression, SyntaxNode root)
        {
            if (expression == null)
            {
                return null;
            }

            var baseUrl = GetBaseUrl(expression.GetProperty(RequireKey.BaseUrl).GetValue(root));

            var packages = TransformPackages(root, expression.GetProperty(RequireKey.Packages)).ToList();

            var paths = TransformPaths(root, expression.GetProperty(RequireKey.Paths));

            var maps = TransformMaps(expression.GetProperty(RequireKey.Map));

            return new RequireConfiguration
                   {
                       BaseUrl = baseUrl,
                       Packages = packages,
                       Paths = paths,
                       Maps = maps
                   };
        }

        private static IDictionary<string, IDictionary<string, string>> TransformMaps(Property maps)
        {
            var result = new Dictionary<string, IDictionary<string, string>>();

            return result;
        }

        private static IDictionary<string, string> TransformPaths(SyntaxNode root, Property paths)
        {
            var result = new Dictionary<string, string>();

            if (paths.Value?.Type != SyntaxNodes.ObjectExpression)
            {
                return result;
            }

            var expr = paths.Value.As<ObjectExpression>();

            foreach (var p in expr.Properties)
            {
                result[p.Key.GetKey()] = p.GetValue(root);
            }

            return result;
        }

        private static IEnumerable<RequirePackage> TransformPackages(SyntaxNode root, Property packages)
        {
            if (packages?.Value?.Type != SyntaxNodes.ArrayExpression)
            {
                return Enumerable.Empty<RequirePackage>();
            }

            var array = packages.Value.As<ArrayExpression>();

            return (from package in array.Elements
                    where package.Type == SyntaxNodes.ObjectExpression
                    select package.As<ObjectExpression>())
                .Select(
                    expr => new RequirePackage
                            {
                                Location = expr.GetProperty("location")?.GetValue(root),
                                Name = expr.GetProperty("name")?.GetValue(root),
                                Main = expr.GetProperty("main")?.GetValue(root)
                            })
                .Where(package =>
                       !string.IsNullOrEmpty(package.Location) ||
                       !string.IsNullOrEmpty(package.Name) ||
                       !string.IsNullOrEmpty(package.Main));
        }

        private static ISet<string> GetBaseUrl(string baseUrl)
        {
            return string.IsNullOrEmpty(baseUrl)
                       ? new SortedSet<string>()
                       : new SortedSet<string> {baseUrl};
        }

        #endregion
    }
}
