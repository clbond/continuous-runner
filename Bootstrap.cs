using System.Linq;
using Autofac;

namespace TestRunner
{
    public class Bootstrap
    {
        public static void Main(string[] args)
        {
            var options = Options.FromArgs(args);

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterInstance(options);

            containerBuilder.RegisterModule<Container>();

            using (var container = containerBuilder.Build())
            {
                var finder = container.Resolve<IScriptFinder>();

                var dependencies = container.Resolve<ISourceDependencies>();

                foreach (var script in finder.GetScripts())
                {
                    dependencies.Add(script);
                }

                foreach (var test in dependencies.GetSuites().SelectMany(testSuite => testSuite.Tests))
                {
                    test.Run();
                }

                var resultWriter = container.Resolve<IResultWriter>();

                resultWriter.Write(dependencies.GetSuites());
            }
        }
    }
}