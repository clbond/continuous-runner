using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Magnum;

namespace ContinuousRunner.Impl
{
    public class ReferenceResolver : IReferenceResolver
    {
        #region Constructors

        public ReferenceResolver([NotNull] IInstanceContext instanceContext)
        {
            Guard.AgainstNull(instanceContext, nameof(instanceContext));
            _instanceContext = instanceContext;
        }

        #endregion

        #region Private members

        private readonly IInstanceContext _instanceContext;

        #endregion

        #region Implementation of IReferenceResolver

        public string Resolve(IScript script, string module)
        {
            if (string.IsNullOrEmpty(module))
            {
                return null;
            }

            if (!module.StartsWith("./", StringComparison.Ordinal))
            {
                return module;
            }

            var segments = module.Split(new[] {'.', '/', '\\'}, StringSplitOptions.RemoveEmptyEntries);

            var qualifiers = new List<string> {_instanceContext.ModuleNamespace};

            var path = script.File.Directory;
            if (path == null)
            {
                throw new TestException($"Cannot resolve module reference: {module}");
            }

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
