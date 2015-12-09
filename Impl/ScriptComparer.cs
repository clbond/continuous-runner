using System;
using System.Collections.Generic;

namespace ContinuousRunner.Impl
{
    public class ScriptComparer : IEqualityComparer<IScript>
    {
        #region Implementation of IEqualityComparer<in IScript>

        public bool Equals(IScript x, IScript y)
        {
            if (x.File == null != (y.File == null))
            {
                return false;
            }

            if (GetFile(x) != GetFile(y))
            {
                return false;
            }

            return string.Compare(x.Content, y.Content, StringComparison.CurrentCulture) == 0;
        }

        public int GetHashCode(IScript obj)
        {
            var f = GetFile(obj);
            if (f != null)
            {
                return f.GetHashCode();
            }

            return obj.Content.GetHashCode();
        }

        #endregion

        private string GetFile(IScript script)
        {
            return script.File?.FullName;
        }
    }
}
