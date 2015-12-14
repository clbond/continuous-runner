using Jint.Parser.Ast;

namespace ContinuousRunner.Frameworks.RequireJs
{
    public class ConfigurationParser : IConfigurationParser
    {
        #region Implementation of IConfigurationParser

        public IRequireConfiguration Parse(ObjectExpression expression)
        {
            return new RequireConfiguration(); // TODO(cbond): Need to implement this parsing logic
        }

        #endregion
    }
}
