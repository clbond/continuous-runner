using System;

namespace ContinuousRunner.Frameworks
{
    [Flags]
    public enum Framework
    {
        /// <summary>
        /// No framework detected
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Jasmine JavaScript unit testing framework
        /// </summary>
        Jasmine = 0x1,

        /// <summary>
        /// NodeJS application
        /// </summary>
        NodeJs = 0x2,

        /// <summary>
        /// 
        /// </summary>
        RequireJs = 0x4
    }
}
