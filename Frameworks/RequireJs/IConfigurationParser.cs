using Jint.Parser.Ast;

namespace ContinuousRunner.Frameworks.RequireJs
{
    public interface IConfigurationParser
    {
        /// <summary>
        /// Parse an object expression, eg <code>{path:{'foo': 'bar'}}</code>, into a <see cref="IRequireConfiguration"/> object
        /// </summary>
        IRequireConfiguration Parse(ObjectExpression expression);
    }
}
