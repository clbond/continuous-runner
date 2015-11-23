using System;

namespace ContinuousRunner
{
    public interface ISourceObserver
    {
        void Added(IScript script);

        event SourceChangedHandler OnAdded;

        void Removed(IScript script);

        event SourceChangedHandler OnRemoved;

        void Changed(IScript script);

        event SourceChangedHandler OnChanged;
    }
}
