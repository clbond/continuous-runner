using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using ContinuousRunner.Data;

namespace ContinuousRunner.Impl
{
    public class TestCollection : ITestCollection
    {
        [Import] private readonly IDeterministicIdentifier _deterministicIdentifier;

        [Import] private readonly IResultFactory _resultFactory;

        private readonly IScript _script;
        
        private readonly IList<TestSuite> _suites = new List<TestSuite>(); 

        public TestCollection(IScript script)
        {
            _script = script;
        }

        public void AddSuite(string description, string definition)
        {
            _suites.Add(
                new TestSuite
                {
                    Name = description,
                    ParentScript = _script,
                    Tests = new List<ITest>(),
                    RawCode = definition
                });
        }

        public void AddTest(string description, string code)
        {
            var lastSuite = _suites.LastOrDefault();
            if (lastSuite == null)
            {
                throw new TestException($"No test suite has been created, cannot define test: {code}");
            }

            var deterministicId = _deterministicIdentifier.GetIdentifier(lastSuite.Name, description);

            var test = new Test
                       {
                           Result = _resultFactory.Indeterminate(),
                           Name = description,
                           Id = deterministicId,
                           Suite = lastSuite,
                           RawCode = code
                       };

            lastSuite.Tests.Add(test);
        }

        public IList<TestSuite> GetSuites()
        {
            return _suites;
        } 
    }
}
