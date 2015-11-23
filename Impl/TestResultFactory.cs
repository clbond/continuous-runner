using System;
using System.Collections.Generic;

namespace ContinuousRunner.Impl
{
    using Data;

    public class TestResultFactory : ITestResultFactory
    {
        #region Implementation of ITestResultFactory

        public TestResult Deleted()
        {
            return new TestResult
            {
                Logs = SingleLog("This test has been deleted from the source"),
                Status = TestStatus.Deleted
            };
        }

        public TestResult FailedToRun(Exception exception)
        {
            return new TestResult
                   {
                       Logs = SingleLog($"Failed to run: {exception}"),
                       Status = TestStatus.Deleted
                   };
        }

        public TestResult Success(IEnumerable<string> logs)
        {
            throw new NotImplementedException();
        }

        public TestResult AssertFailure(Assertion assertion, IEnumerable<string> logs)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private methods

        private static IList<Tuple<DateTime, string>> SingleLog(string log)
        {
            return new[] {Tuple.Create(DateTime.Now, log)};
        }

        #endregion
    }
}
