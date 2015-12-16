using System.Collections.Generic;

namespace ContinuousRunner.Frameworks.RequireJs
{
    public class RequireConfiguration : IRequireConfiguration
    {
        public ISet<string> BaseUrl { get; set; }

        public IDictionary<string, string> Paths { get; set; }

        public IList<RequirePackage> Packages { get; set; }

        public IDictionary<string, IDictionary<string, string>> Maps { get; set; }

        public RequireConfiguration()
        {
            BaseUrl = new HashSet<string>();

            Paths = new Dictionary<string, string>();

            Packages = new List<RequirePackage>();

            Maps = new Dictionary<string, IDictionary<string, string>>();
        }
    }
}
