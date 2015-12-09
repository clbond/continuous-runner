namespace ContinuousRunner
{
    public class ScriptsChangedEvent
    {
        public Operation Operation { get; set; }

        public IScript Script { get; set; }
    }
}
