using System;
using System.Text;

namespace ContinuousRunner.Frameworks.Browser
{
    public class BrowserConsole
    {
        private readonly IScript _script;

        public BrowserConsole(IScript script)
        {
            _script = script;
        }

        #region Methods exposed to the JavaScript runtime

        public void log(params object[] args)
        {
            _script.Logs.Add(Tuple.Create(DateTime.Now, Severity.Info, Format(args)));
        }

        public void error(params object[] args)
        {
            _script.Logs.Add(Tuple.Create(DateTime.Now, Severity.Error, Format(args)));
        }

        public void info(params object[] args)
        {
            log(args);
        }

        #endregion

        #region Private methods

        private static string Format(object[] args)
        {
            var sb = new StringBuilder();

            foreach (var a in args)
            {
                if (sb.Length > 0)
                {
                    sb.Append(" ");
                }

                sb.Append(a?.ToString() ?? "null");
            }

            return sb.ToString();
        }

        #endregion
    }
}
