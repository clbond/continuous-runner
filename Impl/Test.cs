using System;
using System.Threading.Tasks;
using ContinuousRunner.Data;
using ContinuousRunner.Extensions;

namespace ContinuousRunner.Impl
{
    public class Test : ITest
    {
        private readonly IScript _script;

        private readonly Action<TestResultChanged> _stateChanged;

        public Test(IScript script, TestResult initialState, Action<TestResultChanged> stateChanged)
        {
            _script = script;

            _stateChanged = stateChanged;

            Result = initialState;
        }

        #region Implementation of ITest

        public Guid Id { get; set; }

        public string Name { get; set; }

        public TestSuite Suite { get; set; }

        public TestResult Result { get; set; }

        public Task<TestResult> Run()
        {
            var previousResult = Result;

            // TODO(cbond): Obviously, this should actually execute a test using ClearScript here
            Result = new TestResult {Status = TestStatus.Indeterminate};

            var change = new TestResultChanged
                         {
                             Test = this,
                             PreviousResult = previousResult,
                             CurrentResult = Result,
                             Script = _script
                         };

            _stateChanged?.SafeInvoke(change);

            return Task.FromResult(Result);
        }

        #endregion
    }
}