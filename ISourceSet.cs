﻿using System.Collections.Generic;
using System.IO;
using ContinuousRunner.Data;

namespace ContinuousRunner
{
    public interface ISourceDependencies
    {
        void Add(IScript script);

        void Remove(IScript script);

        void Remove(FileInfo fileInfo);

        void Changed(IScript script);

        IScript GetScript(FileInfo fileInfo);

        IScript GetScriptFromModuleReference(string absoluteReference);

        IEnumerable<IScript> GetDependencies(IScript origin);

        IEnumerable<TestSuite> GetSuites();
    }
}