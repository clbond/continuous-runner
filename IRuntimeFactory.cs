namespace ContinuousRunner
{
    public interface IRuntimeFactory<out TEngine>
    {
        TEngine GetRuntime();
    }
}
