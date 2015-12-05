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

            builder.RegisterType<DetectJasmine>()
                   .As<IDetector>();

            builder.RegisterType<DetectNode>()
                   .As<IDetector>();

            builder.RegisterType<DetectRequire>()
                   .As<IDetector>();

            builder.RegisterType<FrameworkDetector>()
                   .AsImplementedInterfaces();

            builder.RegisterType<ModuleReader>()
                   .AsImplementedInterfaces();

            builder.RegisterType<SourceObserver>()
                   .SingleInstance()
                   .AsImplementedInterfaces();

            builder.RegisterType<ScriptLoader>()
                   .AsImplementedInterfaces();
            
            builder.RegisterType<Parser>()
                   .AsImplementedInterfaces();

            builder.RegisterType<SourceSet>()
                   .SingleInstance()
                   .AsImplementedInterfaces();

            builder.RegisterType<ReferenceResolver>()
                   .AsImplementedInterfaces();

            builder.RegisterType<ResultFactory>()
                   .AsImplementedInterfaces();

            builder.RegisterType<ResultObserver>()
                   .SingleInstance()
                   .AsImplementedInterfaces();

            builder.RegisterType<ResultWriter>()
                   .AsImplementedInterfaces();

            builder.RegisterType<SourceObserver>()
                   .SingleInstance()
                   .AsImplementedInterfaces();

            builder.RegisterType<RunQueue>()
                   .SingleInstance()
                   .AsImplementedInterfaces();

            builder.RegisterType<ScriptRunner>()
                   .AsImplementedInterfaces();

            builder.RegisterType<SuiteReader>()
                   .AsImplementedInterfaces();

            builder.RegisterType<Watcher>()
                   .SingleInstance()
                   .AsImplementedInterfaces();
        }
    }
}