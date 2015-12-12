using System;

namespace ContinuousRunner
{
    public interface IDeterministicIdentifier
    {
        Guid GetIdentifier(params string[] identifiers);
    }
}
