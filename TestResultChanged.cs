using ContinuousRunner.Data;

namespace ContinuousRunner
{
    public class TestResultChanged
    {
        public IScript Script { get; set; }

        public ITest Test { get; set; }

        public TestResult PreviousResult { get; set; }

        public TestResult CurrentResult { get; set; }

        #region System.Object overrides

        #region Overrides of Object

        public override string ToString()
        {
            return $"{Script.File.Name}: {Test.Suite.Name} > {Test.Name}: {PreviousResult} -> {CurrentResult}";
        }

        #endregion

        #endregion
    }
}
