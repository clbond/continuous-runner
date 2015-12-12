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

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                   .AsImplementedInterfaces()
                   .Except<Definer>()
                   .OnActivated(args => PropertyInjector.InjectProperties(args.Context, args.Instance));

            builder.Register((c, p) => new Definer(p.TypedAs<IScript>()))
                   .AsSelf()
                   .OnActivated(args => PropertyInjector.InjectProperties(args.Context, args.Instance));
        }
    }
}