using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

using Autofac;

namespace ContinuousRunner.Impl
{
    public class ReferenceResolver : IReferenceResolver
    {
        [Import] private readonly IComponentContext _componentContext;

        [Import] private readonly IInstanceContext _instanceContext;
        
        #region Implementation of IReferenceResolver
        
        public string ResolveToModule(string fromModule, string @ref)
        {
            var systems = _componentContext.Resolve<IEnumerable<IPackageSystem>>();

            var resolved =
                systems.SelectMany(system => system.Resolve(@ref, fromModule))
                       .FirstOrDefault(r => FallbackModuleResolve(r).Exists);

            return resolved ?? @ref;
        }

        public FileInfo Resolve(string fromModule, string @ref)
        {
            var systems = _componentContext.Resolve<IEnumerable<IPackageSystem>>();

            var resolved =
                systems.SelectMany(system => system.Resolve(@ref, fromModule))
                       .FirstOrDefault(f => FallbackModuleResolve(f).Exists);

            var fi = FallbackModuleResolve(resolved ?? @ref);
            if (fi.Exists)
            {
                return fi;
            }

            return null;
        }

        public FileInfo FallbackModuleResolve(string @ref)
        {
            var combined = new List<string>
                           {
                               _instanceContext.ScriptsRoot.FullName,
                               RemoveRoot(@ref)
                           };

            var path = Path.Combine(combined.ToArray());

            return new FileInfo($"{path}.js");
        }

        #endregion

        #region Private methods

        private string RemoveRoot(string @ref)
        {
            var components = @ref.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (components.Length == 0)
            {
                return @ref;
            }

            var root = components.FirstOrDefault();
            if (root == _instanceContext.ModuleNamespace)
            {
                return string.Join("/", components.Skip(1));
            }

            return @ref;
        }

        private string ResolveModule(DirectoryInfo path, string module)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (string.IsNullOrEmpty(module))
            {
                return null;
            }

            if (!module.StartsWith("./", StringComparison.Ordinal))
            {
                return module;
            }

            var segments = module.Split(new[] { '.', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            var qualifiers = new List<string> { _instanceContext.ModuleNamespace };

            while (path != null && path.FullName != _instanceContext.ScriptsRoot.FullName)
            {
                qualifiers.Add(path.Name);

                path = path.Parent;
            }

            qualifiers.Reverse();

            qualifiers.AddRange(segments);

            return string.Join("/", qualifiers);
        }

        #endregion
    }
}
