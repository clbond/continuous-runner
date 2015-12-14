namespace ContinuousRunner.Frameworks
{
    public interface IDetector<out T>
    {
        /// <summary>
        /// Apply some static analysis heuristics to the script <paramref name="script"/>
        /// </summary>
        T Analyze(IScript script);
    }
}