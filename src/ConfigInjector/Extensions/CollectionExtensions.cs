using System;
using System.Collections.Generic;

namespace ConfigInjector.Extensions
{
    internal static class CollectionExtensions
    {
        public static IEnumerable<T> DepthFirst<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> children)
        {
            foreach (var item in source)
            {
                yield return item;
                foreach (var descendant in children(item).DepthFirst(children)) yield return descendant;
            }
        }
    }
}