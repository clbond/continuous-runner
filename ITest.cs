using System;

namespace ContinuousRunner
{
    public interface ITest
    {
        Guid Id { get; }

        string Name { get; }

        string RawCode { get; }

        TestSuite Suite { get; }
        
        TestResult Result { get; set; }
    }
}