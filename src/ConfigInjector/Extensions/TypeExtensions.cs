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
    }
}