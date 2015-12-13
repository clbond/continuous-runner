using System.Reflection;

using Autofac;
using ContinuousRunner.Extractors;
using ContinuousRunner.Impl;

namespace ContinuousRunner
{
    public class ContinuousRunnerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<Publisher>()
                   .As<IPublisher>();

            builder.Register((c, p) => new Definer(p.TypedAs<IScript>()))
                   .As<Definer>()
                   .OnActivated(args => PropertyInjector.InjectProperties(args.Context, args.Instance));

            builder.RegisterType<CachedScripts>()
                   .As<ICachedScripts>()
                   .SingleInstance()
                   .OnActivated(args => PropertyInjector.InjectProperties(args.Context, args.Instance));

            builder.RegisterAssemblyTypes(typeof(ContinuousRunnerModule).Assembly)
                   .Except<Definer>()
                   .Except<Publisher>()
                   .Except<IPublisher>()
                   .Except<CachedScripts>()
                   .Except<ICachedScripts>()
                   .AsImplementedInterfaces()
                   .OnActivated(args => PropertyInjector.InjectProperties(args.Context, args.Instance));
        }
    }
}