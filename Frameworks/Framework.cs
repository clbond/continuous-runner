using System;

namespace ContinuousRunner.Frameworks
{
    [Flags]
    public enum Framework
    {
        None = 0x0,

        /// <summary>
        /// Jasmine JavaScript unit testing framework
        /// </summary>
        Jasmine = 0x1,

        /// <summary>
        /// Does this script make use of NodeJS facilities? (require('module'), etc.)
        /// </summary>
        NodeJs = 0x2,

        /// <summary>
        /// Does this script make use of RequireJS facilities (define(), require([]), etc.)?
        /// </summary>
        RequireJs = 0x4,

        /// <summary>
        /// Is this script written in JavaScript?
        /// </summary>
        JavaScript = 0x8,

        /// <summary>
        /// Does this script use TypeScript that needs to be transformed into JS in order to execute?
        /// </summary>
        TypeScript = 0x16,

        /// <summary>
        /// Is this script written in CoffeeScript?
        /// </summary>
        CoffeeScript = 0x32,

        /// <summary>
        /// Is this code CSharp?
        /// </summary>
        CSharp = 0x64
    }
}
