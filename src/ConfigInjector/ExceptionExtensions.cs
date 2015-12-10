using System;

namespace ConfigInjector
{
    internal static class ExceptionExtensions
    {
        public static T WithData<T>(this T exception, string key, object value) where T : Exception
        {
            exception.Data[key] = value;
            return exception; ;
        }
    }
}