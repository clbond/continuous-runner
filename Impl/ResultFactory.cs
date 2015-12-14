using System;
using System.Collections.Generic;

namespace ContinuousRunner.Impl
{
    using Data;

    public class ResultFactory : IResultFactory
    {
        #region Implementation of ITestResultFactory

        public TestResult Indeterminate()
        {
            return new TestResult
                   {
                       Status = TestStatus.Indeterminate
                   };
        }

        public TestResult Deleted()
        {
            return new TestResult
            {
                Logs = SingleLog(Severity.Error, "Test has been deleted from the source"),
                Status = TestStatus.Deleted
            };
        }

        public TestResult FailedToRun(Exception exception)
        {
            return new TestResult
                   {
                       Logs = SingleLog(Severity.Error, $"Failed to run: {exception}"),
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

        private static IList<Tuple<DateTime, Severity, string>> SingleLog(Severity severity, string log)
        {
            return new[] {Tuple.Create(DateTime.Now, severity, log)};
        }

        #endregion
    }
}
