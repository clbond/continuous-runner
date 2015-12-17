using System.Threading.Tasks;

namespace ContinuousRunner
{
    public interface IPriorityWork
    {
        string Description { get; }

        Priority Priority { get; }

        Task<IExecutionResult> ExecuteAsync();
    }
}
