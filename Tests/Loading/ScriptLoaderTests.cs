﻿using System.IO;
using System.Linq;
using Autofac;
using ContinuousRunner.Tests.Mock;
using FluentAssertions;
using Jint.Parser.Ast;
using Xunit;
using Xunit.Abstractions;

namespace ContinuousRunner.Tests.Loading
{
    public class ScriptLoaderTests : BaseTest
    {
        public ScriptLoaderTests(ITestOutputHelper helper)
            : base(helper)
        { }

        [Fact]
        public void LoadSimpleRequireJsAndJasmineScript()
        {
            using (var container = CreateTypicalContainer(MockFile.TestFile<DirectoryInfo>()))
            {
                const string content =
                    @"define([], function () {
                        describe('Foo', function () {
                          it('Bar', function () {
                            debugger;
                          });
                        });
                      });";

                var fileInfo = MockFile.FromString(content);

                var loader = container.Resolve<IScriptLoader>();

                var script = loader.Load(fileInfo);
                script.Should().NotBeNull();

                // Do a brief verification that we were able to parse the script into a reasonable syntax tree
                script.ExpressionTree.Should().NotBeNull();
                script.ExpressionTree.Root.Type.Should().Be(SyntaxNodes.Program);
                script.ExpressionTree.Root.As<Program>().Should().NotBeNull();
                
                script.Module.Should().NotBeNull();

                var module = script.Module;

                // Module name should be picked up from the filename and root namespace
                module.ModuleName.Should().Be($"{nameof(Tests)}/{Path.GetFileNameWithoutExtension(fileInfo.Name)}");

                // There are no dependencies in the define() statement--
                module.References.Count().Should().Be(0);
            }
        }
    }
}