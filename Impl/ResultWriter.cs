using System.Collections.Generic;
using NLog;

namespace ContinuousRunner.Impl
{
    using Data;

    public class ResultWriter : IResultWriter
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region Implementation of IResultWriter

        public void Write(IEnumerable<TestSuite> testSuites)
        {
            foreach (var suite in testSuites)
            {
                _logger.Info(suite.Name);

                foreach (var test in suite.Tests)
                {
                    _logger.Info("  {0} > {1}", test.Name, test.Result);
                }
            }
        }

        #endregion
    }
}
