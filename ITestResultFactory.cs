using System;
using System.Collections.Generic;

namespace TestRunner
{
    using Data;

    public interface ITestResultFactory
    {
        TestResult Deleted();

        TestResult FailedToRun(Exception exception);

        TestResult Success(IEnumerable<string> logs);

        TestResult AssertFailure(Assertion assertion, IEnumerable<string> logs);
    }
}
