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

        [Import] private readonly IReferenceResolver _referenceResolver;

        [Import] private readonly IRequireConfiguration _requireConfiguration;

        private readonly IDictionary<string, Func<dynamic>> _defines = new Dictionary<string, Func<dynamic>>();

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

        public void Define(string modulePath, string moduleName, Func<dynamic> load)
        {
            if (string.IsNullOrEmpty(moduleName))
            {
                throw new InvalidOperationException("Cannot register a module with no name");
            }

            if (load == null)
            {
                throw new ArgumentNullException(nameof(load));
            }

            _defines[ToAbsolutePath(modulePath, moduleName)] = load;
        }

        public Func<dynamic> GetDefinition(string fromModule, string moduleName)
        {
            var absoluteModule = ToAbsolutePath(fromModule, moduleName);

            var candidates = ResolveCandidates(_requireConfiguration, moduleName).ToList();

            candidates.Insert(0, absoluteModule);

            return candidates.Where(candidate => _defines.ContainsKey(candidate)).Select(candidate => _defines[candidate]).FirstOrDefault();
        }

        #endregion

        #region Private methods

        private string ToAbsolutePath(string modulePath, string moduleName)
        {
            return _referenceResolver.ResolveToModule(modulePath, moduleName);
        }

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