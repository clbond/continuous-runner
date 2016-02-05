namespace ContinuousRunner.Frameworks.jQuery
{
    public class Detect : IDetector<Framework>
    {
        #region Implementation of IDetector<out Framework>

        public Framework Analyze(IProjectSource source)
        {
            return Framework.jQuery; // FIXME(cbond): Detect usage of jQuery
        }

        #endregion
    }
}
