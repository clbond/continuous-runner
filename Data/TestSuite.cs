using System;
using System.Collections.Generic;

namespace ContinuousRunner.Data
{
    public class TestSuite
    {
        #region Public properties

        public string Name { get; set; }

        public string RawCode { get; set; }

        public IList<ITest> Tests { get; set; }

        public IScript ParentScript
        {
            get
            {
                return _parentReference.Target as IScript;
            }
            set
            {
                _parentReference = new WeakReference(value);
            }
        }

        #endregion

        #region Private members

        private WeakReference _parentReference;

        #endregion
    }
}