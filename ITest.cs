using System;

namespace ContinuousRunner
{
    using Data;

    public interface ITest
    {
        Guid Id { get; }

        string Name { get; }

        string RawCode { get; }

        TestSuite Suite { get; }

        /// <summary>
        /// The result of this test the last time it was run, or null if it has not been run
        /// </summary>
        TestResult Result { get; set; }
    }
}