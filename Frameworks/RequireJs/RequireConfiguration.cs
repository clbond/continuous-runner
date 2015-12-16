using System.Collections.Generic;

namespace ContinuousRunner.Frameworks.RequireJs
{
    public class RequireConfiguration : IRequireConfiguration
    {
        public string BaseUrl { get; set; }

        public IDictionary<string, string> Paths { get; set; }

        public IList<RequirePackage> Packages { get; set; }

        public IDictionary<string, IDictionary<string, string>> Maps { get; set; }
    }
}
