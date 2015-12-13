using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

using Autofac;
using Autofac.Core;

namespace ContinuousRunner
{
    public static class PropertyInjector
    {
        public static void InjectProperties(IComponentContext context, object instance)
        {
            var properties = instance.GetType().GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var fieldInfo in properties)
            {
                var propertyType = fieldInfo.FieldType;
                if (propertyType.IsValueType || !context.IsRegistered(propertyType))
                {
                    continue;
                }

                if (HasImportAttribute(fieldInfo))
                {
                    if (fieldInfo.GetValue(instance) != null)
                    {
                        continue; // do not overwrite existing non-null values
                    }

                    var obj = context.Resolve(propertyType);
                    if (obj == null)
                    {
                        throw new DependencyResolutionException(
                            $"Unable to resolve dependency import on {instance.GetType()} -> {fieldInfo}");
                    }

                    fieldInfo.SetValue(instance, obj);
                }
            }
        }

        private static bool HasImportAttribute(MemberInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes(typeof(ImportAttribute)).Any();
        }
    }
}