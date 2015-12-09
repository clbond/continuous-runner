using System.Reflection;
using Autofac;
using ContinuousRunner.Extractors;

namespace ContinuousRunner
{
    using Frameworks;
    using Frameworks.Detectors;
    using Impl;

    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                   .AsImplementedInterfaces();

            builder.RegisterType<Publisher>().AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}