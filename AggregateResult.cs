using System;
using System.Collections.Generic;
using System.Linq;

namespace ContinuousRunner
{
    public class AggregateResult : IExecutionResult
    {
        private readonly IExecutionResult[] _results;

        public AggregateResult(IEnumerable<IExecutionResult> results)
        {
            _results = results.ToArray();
        }

        #region Implementation of IExecutionResult

        public Status Status
        {
            get
            {
                if (_results.Any(s => s.Status == Status.Failed))
                {
                    return Status.Failed;
                }

                return _results.Max(r => r.Status);
            }
        }

        public IList<Tuple<DateTime, Severity, string>> Logs
        {
            get { return _results.SelectMany(r => r.Logs).ToList(); }
        }

        #endregion
    }
}
