using System.Collections.Generic;

namespace ContinuousRunner
{
	public interface IPackageSystem
	{
		/// <summary>
		/// Using configuration gleaned about this packaging system (eg. from searching for calls to requirejs.config()), attempt
		/// to resolve the module <paramref name="moduleName" /> into a file on the local filesystem. This should work for modules
		/// declared in the project, or modules imported from third-party libraries.
		/// </summary>
		IEnumerable<string> Resolve(string moduleName, string fromModule);
	}
}