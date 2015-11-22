using System;
using System.Collections.Generic;

namespace TestRunner
{
    public class ReferenceResolver : IReferenceResolver
    {
        private readonly Options _options;

        public ReferenceResolver(Options options)
        {
            _options = options;
        }

        #region Implementation of IReferenceResolver

        public string Resolve(IScript script, string require)
        {
            if (string.IsNullOrEmpty(require))
            {
                return null;
            }

            if (!require.StartsWith("./", StringComparison.Ordinal))
            {
                return require;
            }

            var segments = require.Split(new[] {'.', '/', '\\'}, StringSplitOptions.RemoveEmptyEntries);

            var qualifiers = new List<string> {_options.ModuleNamespace};

            var path = script.File.Directory;
            if (path == null)
            {
                throw new TestException($"Cannot resolve module reference: {require}");
            }

            while (path != null && path.FullName != _options.Root.FullName)
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
