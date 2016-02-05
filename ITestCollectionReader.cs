namespace ContinuousRunner
{
    public interface ITestCollectionReader
    {
        /// <summary>
        /// Extract a collection of test suites and tests from the script, <paramref name="script"/>
        /// </summary>
        ITestCollection DefineTests(IPackageSystem packageSystem, IScript script);
    }
}
