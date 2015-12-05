using Autofac;

namespace ContinuousRunner.Tests
{
    public static class Container
    {
        public static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            var instanceContext = new TestInstance();
            
            builder.RegisterInstance(instanceContext).As<IInstanceContext>();

            builder.RegisterModule<Module>();

            return builder.Build();
        }
    }
}
