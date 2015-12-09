namespace ContinuousRunner
{
    public class SourceChangedEvent
    {
        public Operation Operation { get; set; }

        public IScript Script { get; set; }
    }
}
