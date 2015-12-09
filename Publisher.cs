using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;

namespace ContinuousRunner
{
    public class Publisher : IPublisher
    {
        public Publisher(ILifetimeScope scope)
        {
            _scope = scope;
        }

        private readonly ILifetimeScope _scope;

        public void Publish<T>(T @event) where T : class
        {
            var exceptions = new List<Exception>();

            foreach (dynamic handler in ResolveHandlers<T>())
            {
                try
                {
                    handler.Handle((dynamic) @event);
                }
                catch (Exception exception)
                {
                    exceptions.Add(exception);
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        public IEnumerable<dynamic> ResolveHandlers<T>() where T : class
        {
            return GetConcreteHandlers<T>().Union(GetInterfaceHandlers<T>());
        }

        private IEnumerable<dynamic> GetConcreteHandlers<T>() where T : class
        {
            return (IEnumerable<dynamic>) _scope.Resolve(MakeHandlerType(typeof(T)));
        }

        private IEnumerable<dynamic> GetInterfaceHandlers<T>() where T : class
        {
            var implementedInterfaces = typeof (T).GetTypeInfo().ImplementedInterfaces;

            var resolved = implementedInterfaces.SelectMany(@interface => (IEnumerable<ISubscription<T>>) _scope.Resolve(MakeHandlerType(@interface)));

            return resolved.Distinct();
        }

        private static Type MakeHandlerType(Type type)
        {
            return typeof (IEnumerable<>).MakeGenericType(typeof (ISubscription<>).MakeGenericType(type));
        }
    }
}