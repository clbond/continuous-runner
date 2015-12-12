﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using ContinuousRunner.Data;
using ContinuousRunner.Impl;

namespace ContinuousRunner.Extractors
{
    public class Definer
    {
        [Import] private readonly IDeterministicIdentifier _deterministicIdentifier;

        [Import] private readonly IResultFactory _resultFactory;

        private readonly IScript _script;
        
        private readonly IList<TestSuite> _suites = new List<TestSuite>(); 

        public Definer(IScript script)
        {
            _script = script;
        }

        public void AddSuite(string description)
        {
            _suites.Add(
                new TestSuite
                {
                    Name = description,
                    ParentScript = _script,
                    Tests = new List<ITest>()
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
                           Result = _resultFactory.InitialState(),
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
