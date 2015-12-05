using System;
using ContinuousRunner.Data;

namespace ContinuousRunner.Impl
{
    public class Test : ITest
    {
        private readonly IScript _script;

        public Test(IScript script, string definitionCode)
        {
            Id = Guid.NewGuid();

            _script = script;

            RawCode = definitionCode;
        }

        #region Implementation of ITest

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string RawCode { get; set; }

        public TestSuite Suite { get; set; }

        public TestResult Result { get; set; }
        
        #endregion
    }
}