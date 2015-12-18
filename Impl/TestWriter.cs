using System.Collections.Generic;
using NLog;

namespace ContinuousRunner.Impl
{
    public class TestWriter : ITestWriter
    {
        #region Implementation of IResultWriter

        public void Write(IEnumerable<TestSuite> testSuites)
        {
            var logger = LogManager.GetCurrentClassLogger();

            foreach (var suite in testSuites)
            {
                logger.Info(suite.Name);

                foreach (var test in suite.Tests)
                {
                    logger.Info($"  {test.Name} > {test.Result}");
                }
            }
        }

        #endregion
    }
}
