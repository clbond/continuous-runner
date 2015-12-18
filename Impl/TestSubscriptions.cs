using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Autofac;
using NLog;

namespace ContinuousRunner.Impl
{
    using Work;

    public class TestSubscriptions : ISubscription<SourceChangedEvent>,
                                     ISubscription<TestResult>
    {
        [Import] private readonly IComponentContext _componentContext;

        [Import] private readonly IConcurrentExecutor _executor;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region Implementation of ISubscription<in SourceChangedEvent>

        public void Handle(SourceChangedEvent @event)
        {
            _logger.Debug($"Source file changed: {@event}; queueing run");

            if (@event.SourceFile is IScript)
            {
                var script = (IScript) @event.SourceFile;

                Task.Run(() =>
                         {
                             var work = _componentContext.Resolve<ExecuteScriptWork>(new TypedParameter(typeof(IScript), script));

                             var t = _executor.ExecuteAsync(work);

                             t.Wait();
                         });
            }
            else if (@event.SourceFile is IClass)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Implementation of ISubscription<in TestResult>

        public void Handle(TestResult @event)
        {
            var test = @event.Test;

            var s = test?.Suite?.ParentScript;
            if (s != null)
            {
                _logger.Debug($"Test result changed: {test}: {@event}");
            }
        }

        #endregion
    }
}
