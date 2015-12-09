using System;

using ContinuousRunner.Data;

namespace ContinuousRunner.Impl
{
    public class Test : ITest
    {
        private WeakReference _parentSuite;

        public Test(IScript script, TestSuite suite, string definitionCode)
        {
            Id = Guid.NewGuid();
            
            RawCode = definitionCode;
        }

        #region Implementation of ITest

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string RawCode { get; set; }

        public TestResult Result { get; set; }

        public TestSuite Suite
        {
            get
            {
                if (_parentSuite.IsAlive == false)
                {
                    throw new TestException("Parent suite has been disposed");
                }

                return _parentSuite.Target as TestSuite;
            }
            set
            {
                _parentSuite = new WeakReference(value);
            }
        }

        #endregion
    }
}