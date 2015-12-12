using System;
using System.Reflection;

namespace ConfigInjector.Infrastructure.ValueParsers
{
    public class AnyOldParseMethodValueParser : IValueParser
    {
        public int SortOrder
        {
            get { return int.MaxValue - 1; }
        }

        public bool CanParse(Type settingValueType)
        {
            return GetParseMethod(settingValueType) != null;
        }

        public object Parse(Type settingValueType, string settingValueString)
        {
            var parseMethod = settingValueType.GetMethod("Parse", new[] { typeof(string) });
            var result = parseMethod.Invoke(null, new object[] { settingValueString });
            return result;
        }

        private static MethodInfo GetParseMethod(Type settingValueType)
        {
            var parseMethod = settingValueType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
            return parseMethod;
        }
    }
}