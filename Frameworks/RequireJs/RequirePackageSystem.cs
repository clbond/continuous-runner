using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Autofac;

namespace ContinuousRunner.Frameworks.RequireJs
{
    public class RequirePackageSystem : IPackageSystem
    {
        [Import] private readonly IComponentContext _componentContext;

        #region Implementation of IPackageSystem

        public IEnumerable<string> Resolve(string moduleName, string fromModule)
        {
            var configuration = _componentContext.ResolveOptional<IRequireConfiguration>();
            if (configuration == null)
            {
                return Enumerable.Empty<string>();
            }

            var candidates = ResolveCandidates(configuration, moduleName);

            return candidates.Select(c => ApplyMaps(configuration, fromModule, c));
        }

        #endregion

        #region Private methods

        private static string ApplyMaps(IRequireConfiguration configuration, string fromModule, string @ref)
        {
            if (configuration.Maps == null)
            {
                return @ref;
            }

            if (configuration.Maps.ContainsKey(fromModule))
            {
                @ref = ApplyMaps(configuration.Maps[fromModule], @ref);
            }

            if (configuration.Maps.ContainsKey("*"))
            {
                @ref = ApplyMaps(configuration.Maps["*"], @ref);
            }

            return @ref;
        }

        private static string ApplyMaps(IDictionary<string, string> dictionary, string @ref)
        {
            return dictionary.ContainsKey(@ref)
                       ? dictionary[@ref]
                       : @ref;
        }

        private static IEnumerable<string> ResolveCandidates(IRequireConfiguration configuration, string moduleName)
        {
            foreach (var r in GetPath(configuration, moduleName))
            {
                yield return r;
            }

            foreach (var p in GetPackages(configuration, moduleName))
            {
                yield return p;
            }
        }

        private static IEnumerable<string> GetPackages(IRequireConfiguration configuration, string moduleName)
        {
            return from p in configuration.Packages where moduleName.StartsWith(p.Name, StringComparison.InvariantCulture) select p.Location;
        }

        private static IEnumerable<string> GetPath(IRequireConfiguration configuration, string moduleName)
        {
            return from path in configuration.Paths where path.Key == moduleName select path.Value;
        }

        #endregion
    }
}