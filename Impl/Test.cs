 using System;

using ContinuousRunner.Data;

namespace ContinuousRunner.Impl
{
    public class Test : ITest
    {
        public Test()
        {
            Id = Guid.NewGuid();
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
                return _parentSuite.Target as TestSuite;
            }
            set
            {
                _parentSuite = new WeakReference(value);
            }
        }

        #endregion

        #region Private members

        private WeakReference _parentSuite;

        #endregion
    }
}