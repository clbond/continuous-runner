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
                TestSuite target;
                if (_parentSuite.TryGetTarget(out target))
                {
                    return target;
                }

                throw new ObjectDisposedException("Parent suite has been disposed");
            }
            set
            {
                _parentSuite = new WeakReference<TestSuite>(value);
            }
        }

        #endregion

        #region System.Object overrides

        #region Overrides of Object

        public override string ToString()
        {
            return $"{Name} ({Id.ToString()})";
        }

        #endregion

        #endregion

        #region Private members

        private WeakReference<TestSuite> _parentSuite;

        #endregion
    }
}