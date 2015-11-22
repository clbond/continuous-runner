using System;
using System.Collections.Generic;

namespace TestRunner
{
    public static class TestResultFactory
    {
        public static TestResult Deleted()
        {
            return new TestResult
            {
                Logs = SingleLog("This test has been deleted from the source"),
                Status = TestStatus.Deleted
            };
        }

        public static TestResult FailedToRun(Exception exception)
        {
            return new TestResult
            {
                Logs = SingleLog($"Failed to run: {exception}"),
                Status = TestStatus.Deleted
            };
        }

        private static IList<Tuple<DateTime, string>> SingleLog(string log)
        {
            return new[] {Tuple.Create(DateTime.Now, log)};
        }
    }
}
