namespace ContinuousRunner
{
    public interface IResultObserver
    {
        void ResultChanged(TestResultChanged changedEvent);

        event TestResultChangedHandler OnResultChanged;
    }
}
