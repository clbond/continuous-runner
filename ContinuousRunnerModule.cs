using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.Scanning;
using ContinuousRunner.Frameworks.RequireJs;
using ContinuousRunner.Impl;
using Microsoft.ClearScript;

namespace ContinuousRunner
{
    public class ContinuousRunnerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var singleInstanceRegisters = new[]
                                          {
                                              typeof (CachedScripts),
                                              typeof (ScriptCollection),
                                              typeof (RunQueue)
                                          };

            foreach (var r in singleInstanceRegisters)
            {
                builder.RegisterType(r)
                       .AsImplementedInterfaces()
                       .SingleInstance()
                       .OnActivating(ActivateInject);
            }

            builder.Register((c, p) => new TestCollection(p.TypedAs<IScript>()))
                   .AsImplementedInterfaces()
                   .OnActivating(ActivateInject);

            var completeRegister = builder.RegisterAssemblyTypes(typeof(ContinuousRunnerModule).Assembly)
                   .AsImplementedInterfaces()
                   .OnActivating(ActivateInject);

            builder.RegisterType<RequireConfigurationLoader>()
                   .AsImplementedInterfaces()
                   .OnActivating(ActivateInject);

            var except = typeof(Autofac.RegistrationExtensions).GetMethod("Except",
                new[] {typeof(IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>) });

            foreach (var r in singleInstanceRegisters)
            {
                var method = except.MakeGenericMethod(r);

                completeRegister =
                    method.Invoke(completeRegister, new object[] {completeRegister}) as
                    IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>;
            }

            builder.Register(
                c => c.Resolve<IRequireConfigurationLoader>().Load(c.Resolve<IScriptCollection>().GetScriptFiles()))
                   .As<IRequireConfiguration>()
                   .SingleInstance()
                   .OnActivating(ActivateInject);

            builder.Register((c, p) =>
                             {
                                 var parameters = p.ToArray();

                                 return new RequireDefine(parameters.TypedAs<ScriptEngine>(),
                                                          parameters.TypedAs<IScript>());
                             })
                   .OnActivating(ActivateInject);
        }

        private static void ActivateInject<T>(IActivatingEventArgs<T> args)
        {
            PropertyInjector.InjectProperties(args.Context, args.Instance);
        }
    }
}