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
        /// Raw JavaScript content of this script
        /// </summary>
        string Content { get; }

        /// <summary>
        /// A simple description of this script for display purposes
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The number of tests defined in this script (may change on reload)
        /// </summary>
        int TestCount { get; }

        /// <summary>
        /// Reload the contents of this script since it has changed on disk
        /// </summary>
        void Reload();
    }
}