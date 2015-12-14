using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ContinuousRunner.Impl
{
    public class ReferenceResolver : IReferenceResolver
    {
        [Import]
        private readonly IInstanceContext _instanceContext;
        
        #region Implementation of IReferenceResolver

        public string Resolve(IScript script, string module)
        {
            if (script.File == null)
            {
                throw new InvalidOperationException(
                    $"Cannot parse module reference from script: {script} -- no associated file");
            }

            return ResolveModule(script.File.Directory, module);
        }

        public string Resolve(string sourceModule, string module)
        {
            var path = Path.GetDirectoryName(Path.Combine(_instanceContext.ScriptsRoot.FullName, RemoveRoot(sourceModule)));

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var root = new DirectoryInfo(path);

            return ResolveModule(root, module);
        }

        public FileInfo ModuleReferenceToFile(string @ref)
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
