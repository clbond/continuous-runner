using Autofac;

namespace ContinuousRunner.Tests
{
    public static class Container
    {
        public static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<TestInstance>()
                   .SingleInstance()
                   .As<IInstanceContext>();

            builder.RegisterModule<ContinuousRunnerModule>();

            return builder.Build();
        }
    }
}
