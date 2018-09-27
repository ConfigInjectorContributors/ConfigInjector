using System;

namespace ConfigInjector.Extensions
{
    internal static class TypeExtensions
    {
        internal static bool IsInstantiable(this Type type)
        {
            if (type.IsInterface) return false;
            if (type.IsAbstract) return false;
            if (type.ContainsGenericParameters) return false;
            return true;
        }

        internal static bool IsAssignableTo<TTarget>(this Type type)
        {
            return typeof(TTarget).IsAssignableFrom(type);
        }

        internal static bool IsClosedTypeOf(this Type type, Type genericType)
        {
            var baseType = type.BaseType;
            if (baseType == null) return false;
            if (!baseType.IsGenericType) return false;
            var isClosedTypeOf = (baseType.GetGenericTypeDefinition() == genericType);
            return isClosedTypeOf;
        }
    }
}