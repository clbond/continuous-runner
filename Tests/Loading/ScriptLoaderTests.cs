using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task LoadSimpleRequireJsAndJasmineScript()
        {
            using (var container = CreateTypicalContainer(MockFile.TempFile<DirectoryInfo>()))
            {
                const string content =
                    @"define([], function () {
                        describe('Foo', function () {
                          it('Bar', function () {
                            console.log('Test output');
                          });
                        });
                      });";

                var fileInfo = container.Resolve<IMockFile>().FromString("js", content);

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

                var tests = script.Suites.ToArray();

                var testWriter = container.Resolve<ITestWriter>();

                testWriter.Write(tests);

                var runner = container.Resolve<IRunQueue>();

                runner.Push(script);

                var results = await runner.RunAllAsync().ConfigureAwait(false);

                testWriter.Write(tests);
            }
        }
    }
}
