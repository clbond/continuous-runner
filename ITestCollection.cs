using System.Collections.Generic;

namespace ContinuousRunner
{
    public interface ITestCollection
    {
        void AddSuite(string description, string definition);

        void AddTest(string description, string code);

        IList<TestSuite> GetSuites();
    }
}