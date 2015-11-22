using Autofac;

namespace TestRunner
{
    public class Container : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ModuleReader>()
                   .AsImplementedInterfaces();

            builder.RegisterType<ScriptFinder>()
                   .AsImplementedInterfaces();

            builder.RegisterType<ScriptLoader>()
                   .AsImplementedInterfaces();

            builder.RegisterType<ScriptParser>()
                   .AsImplementedInterfaces();

            builder.RegisterType<SourceDependencies>()
                   .SingleInstance()
                   .AsImplementedInterfaces();

            builder.RegisterType<ResultWriter>()
                   .AsImplementedInterfaces();

            builder.RegisterType<TestQueue>()
                   .SingleInstance()
                   .AsImplementedInterfaces();

            builder.RegisterType<Watcher>()
                   .SingleInstance()
                   .AsImplementedInterfaces();
        }
    }
}