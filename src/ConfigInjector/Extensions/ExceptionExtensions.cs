using System;

namespace ConfigInjector.Extensions
{
    internal static class ExceptionExtensions
    {
        public static T WithData<T>(this T exception, string key, object value) where T : Exception
        {
            exception.Data[key] = value;
            return exception;
            ;
        }
    }
}