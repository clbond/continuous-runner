using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace ContinuousRunner.Work
{
    public class ExecuteScriptWork : IPriorityWork
    {
        [Import] private readonly IRunner<IScript> _runner;

        private readonly IScript _script;

        public ExecuteScriptWork(IRunner<IScript> runner, IScript script, string description)
        {
            if (runner == null)
            {
                throw new ArgumentNullException(nameof(runner));
            }

            if (script == null)
            {
                throw new ArgumentNullException(nameof(script));
            }

            _runner = runner;

            _script = script;

            Description = description;
        }

        #region Implementation of IPriorityWork

        public string Description { get; set; }

        public Priority Priority { get; set; }

        public async Task<IExecutionResult> ExecuteAsync()
        {
            var tasks = _runner.RunAsync(_script).ToList();

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            return new AggregateResult(results);
        }

        #endregion
    }
}
