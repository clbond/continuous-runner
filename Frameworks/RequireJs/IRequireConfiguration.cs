using System.Collections.Generic;

namespace ContinuousRunner.Frameworks.RequireJs
{
    public interface IRequireConfiguration
    {
        IDictionary<string, string> Paths { get; }

        IList<RequirePackage> Packages { get; }

        IDictionary<string, IDictionary<string, string>> Maps { get; } 
    }
}
