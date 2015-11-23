using System;
using System.Collections.Generic;

namespace ContinuousRunner.Extensions
{
    public static class DelegateExtensions
    {
        /// <summary>
        /// Invoke an EventHandler object in a way that is very safe, so that if any of the handlers throw
        /// an exception, it will continue invoking any additional callbacks that are attached to the handler,
        /// and then it will return any exceptions encountered during execution of the callbacks, instead of
        /// throwing them. So you should inspect the return value and log the results.
        /// </summary>
        /// <typeparam name="T">The type of event handler we will invoke</typeparam>
        /// <param name="eventHandler">The event handler callbacks have been registered on</param>
        /// <param name="args">Arguments that we are going to pass to the callback events</param>
        public static IList<Exception> SafeInvoke<T>(this T eventHandler, params object[] args)
        {
            var exceptions = new List<Exception>();

            var handler = eventHandler as EventHandler;
            if (handler != null) // no handlers attached?
            {
                foreach (var invoke in handler.GetInvocationList())
                {
                    try
                    {
                        invoke.DynamicInvoke(args);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            }

            return exceptions;
        }
    }
}
