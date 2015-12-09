using System.Collections.Generic;
using NLog;

namespace ContinuousRunner.Impl
{
    using Data;

    public class ResultWriter : IResultWriter
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
                    logger.Info("  {0} > {1}", test.Name, test.Result);
                }
            }
        }

        #endregion
    }
}
