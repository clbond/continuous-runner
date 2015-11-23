using System;
using System.Collections.Generic;

namespace ContinuousRunner
{
    using Data;

    public interface IResultFactory
    {
        TestResult Deleted();

        TestResult FailedToRun(Exception exception);

        TestResult Success(IEnumerable<string> logs);

        TestResult AssertFailure(Assertion assertion, IEnumerable<string> logs);
    }
}
