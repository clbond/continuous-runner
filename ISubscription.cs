namespace ContinuousRunner
{
    public interface ISubscription<in T> where T : class
    {
        void Handle(T @event);
    }
}
