using System.Collections.Generic;
using System.Linq;
using Jint.Parser.Ast;

namespace ContinuousRunner.Frameworks.RequireJs
{
    using Extensions;

    public class ConfigurationParser : IConfigurationParser
    {
        #region Implementation of IConfigurationParser

        public IRequireConfiguration Parse(SyntaxNode root, ObjectExpression expression)
        {
            return new RequireConfiguration
                   {
                       BaseUrl = expression.GetProperty(RequireKey.BaseUrl).GetValue(root),
                       Packages = TransformPackages(root, expression.GetProperty(RequireKey.Packages)).ToList(),
                       Paths = TransformPaths(root, expression.GetProperty(RequireKey.Paths)),
                       Maps = TransformMaps(expression.GetProperty(RequireKey.Map))
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
            if (packages.Value?.Type != SyntaxNodes.ArrayExpression)
            {
                yield break;
            }

            var array = packages.Value.As<ArrayExpression>();

            foreach (var expr in from package in array.Elements where package.Type == SyntaxNodes.ObjectExpression select package.As<ObjectExpression>())
            {
                yield return new RequirePackage
                             {
                                 Location = expr.GetProperty("location")?.GetValue(root),
                                 Name = expr.GetProperty("name")?.GetValue(root),
                                 Main = expr.GetProperty("main")?.GetValue(root)
                             };
            }
        }

        #endregion
    }
}
