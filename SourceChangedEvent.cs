namespace ContinuousRunner
{
    public class SourceChangedEvent
    {
        public Operation Operation { get; set; }

        public IProjectSource SourceFile { get; set; }
    }
}
