namespace ContinuousRunner
{
    public interface IPublisher
    {
        void Publish<T>(T @event) where T : class;
    }
}
