using System;
using System.IO;
using System.Collections.Generic;

namespace ContinuousRunner
{
    using Data;

    public interface IScript : IComparable<IScript>
    {
        /// <summary>
        /// The file associated with this JavaScript item
        /// </summary>
        FileInfo File { get; }

        /// <summary>
        /// An abstract syntax tree of a parsed JavaScript file
        /// </summary>
        SyntaxTree SyntaxTree { get; }

        /// <summary>
        /// The module definition extracted from the JavaScript code (define() statement details)
        /// </summary>
        ModuleDefinition Module { get; }

        /// <summary>
        /// Get a collection of test suites defined in this script
        /// </summary>
        IEnumerable<TestSuite> Suites { get; }

        /// <summary>
        /// Get a collection of scripts that this one references as dependencies
        /// </summary>
        IEnumerable<IScript> Requires { get; } 

        /// <summary>
        /// Reload the contents of this script since it has changed on disk
        /// </summary>
        void Reload();

        /// <summary>
        /// An event that will be emitted when this script gets reloaded
        /// </summary>
        event SourceChangedHandler Changed;
    }
}