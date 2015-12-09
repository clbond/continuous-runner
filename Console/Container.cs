using Autofac;

namespace ContinuousRunner.Console
{
    public static class Container
    {
        public static IContainer Build(IInstanceContext options)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(options).As<IInstanceContext>();

            builder.RegisterModule<ContinuousRunnerModule>();

            return builder.Build();
        }
    }
}
