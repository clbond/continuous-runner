using Autofac;
using Autofac.Core;
using ContinuousRunner.Frameworks.RequireJs;
using ContinuousRunner.Impl;

namespace ContinuousRunner
{
    public class ContinuousRunnerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            
            builder.RegisterType<Publisher>()
                   .As<IPublisher>()
                   .OnActivated(ActivateInject);

            builder.Register((c, p) => new TestCollection(p.TypedAs<IScript>()))
                   .As<ITestCollection>()
                   .OnActivated(ActivateInject);

            builder.RegisterType<CachedScripts>()
                   .As<ICachedScripts>()
                   .SingleInstance()
                   .OnActivated(ActivateInject);

            builder.RegisterType<RunQueue>()
                   .AsImplementedInterfaces()
                   .SingleInstance()
                   .OnActivated(ActivateInject);

            builder.Register(
                c => c.Resolve<IRequireConfigurationLoader>().Load(c.Resolve<IScriptCollection>().GetScriptFiles()))
                   .As<IRequireConfiguration>()
                   .SingleInstance();

            builder.RegisterAssemblyTypes(typeof(ContinuousRunnerModule).Assembly)
                   .Except<TestCollection>()
                   .Except<Publisher>()
                   .Except<CachedScripts>()
                   .Except<RunQueue>()
                   .Except<RequireConfiguration>()
                   .AsImplementedInterfaces()
                   .OnActivated(ActivateInject);
        }

        private static void ActivateInject<T>(IActivatedEventArgs<T> args)
        {
            PropertyInjector.InjectProperties(args.Context, args.Instance);
        }
    }
}