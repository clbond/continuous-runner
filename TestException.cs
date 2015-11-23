using System;

namespace ContinuousRunner
{
    public class TestException : ApplicationException
    {
        public TestException(string msg, Exception innerException = null)
            : base(msg, innerException)
        {}
    }
}
