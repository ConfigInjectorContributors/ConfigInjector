using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigInjector.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> source) where T : class
        {
            return source.Where(item => item != null);
        }

        public static bool None<T>(this IEnumerable<T> source)
        {
            return !source.Any();
        }

        public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return !source.Any(predicate);
        }
    }
}