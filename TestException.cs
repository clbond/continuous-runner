using System;
using System.Runtime.Serialization;

namespace ContinuousRunner
{
    [Serializable]
    public class TestException : ApplicationException
    {
        public TestException(string msg, Exception innerException = null)
            : base(msg, innerException)
        {}

        protected TestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}
    }
}
