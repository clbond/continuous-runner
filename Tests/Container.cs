using System;
using Autofac;

namespace ContinuousRunner.Tests
{
    public static class Container
    {
        public static IContainer CreateContainer(Action<ContainerBuilder> build = null)
        {
            var builder = new ContainerBuilder();
            
            builder.RegisterModule<ContinuousRunnerModule>();

            build?.Invoke(builder);

            return builder.Build();
        }
    }
}
