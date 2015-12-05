using System;
using System.Collections.Generic;
using System.Diagnostics;

using NLog;

namespace ContinuousRunner.Extractors
{
    using Data;
    using Impl;

    public class TestCollection
    {
        public IScript Script { get; set; }

        public readonly IList<TestSuite> Suites = new List<TestSuite>();

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private TestSuite _currentSuite;

        public void AddSuite(string description, string definition)
        {
            Debugger.Break();

            _currentSuite = new TestSuite
                            {
                                Name = description,
                                Tests = new List<ITest>()
                            };

            Suites.Add(_currentSuite);
        }

        public void AddTest(string description, string definition)
        {
            _currentSuite?.Tests.Add(
                new Test(Script, definition)
                {
                    Id = Guid.NewGuid(),
                    Name = description,
                    Suite = _currentSuite
                });
        }
    }
}
