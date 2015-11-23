using Autofac;

namespace ContinuousRunner
{
    using Impl;

    public class Module : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ModuleReader>()
                   .AsImplementedInterfaces();

            builder.RegisterType<SourceMutator>()
                   .SingleInstance()
                   .AsImplementedInterfaces();

            builder.RegisterType<SourceObserver>()
                   .AsImplementedInterfaces();

            builder.RegisterType<ScriptLoader>()
                   .AsImplementedInterfaces();

            builder.RegisterType<ScriptParser>()
                   .AsImplementedInterfaces();

            builder.RegisterType<SourceSet>()
                   .SingleInstance()
                   .AsImplementedInterfaces();

            builder.RegisterType<ReferenceResolver>()
                   .AsImplementedInterfaces();

            builder.RegisterType<ResultWriter>()
                   .AsImplementedInterfaces();

            builder.RegisterType<RunQueue>()
                   .SingleInstance()
                   .AsImplementedInterfaces();

            builder.RegisterType<Watcher>()
                   .SingleInstance()
                   .AsImplementedInterfaces();
        }
    }
}