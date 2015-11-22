using Autofac;

namespace TestRunner
{
    public class RunnerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ScriptLoader>()
                   .AsImplementedInterfaces();
        }
    }
}
