using System.Reflection;

using Autofac;
using ContinuousRunner.Extractors;

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

            builder.RegisterAssemblyTypes(typeof(ContinuousRunnerModule).Assembly)
                   .Except<Definer>()
                   .Except<Publisher>()
                   .Except<IPublisher>()
                   .AsImplementedInterfaces()
                   .OnActivated(args => PropertyInjector.InjectProperties(args.Context, args.Instance));
        }
    }
}