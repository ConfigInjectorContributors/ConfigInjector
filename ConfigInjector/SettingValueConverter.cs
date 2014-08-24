using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfigInjector.ValueParsers;

namespace ConfigInjector
{
    internal class SettingValueConverter
    {
        private readonly IValueParser[] _valueParsers;

        public SettingValueConverter(params IValueParser[] customValueParsers)
        {
            _valueParsers = LoadValueParsers()
                .Union(customValueParsers)
                .OrderBy(vp => vp.SortOrder)
                .ToArray();
        }

        private static IEnumerable<IValueParser> LoadValueParsers()
        {
            var parsers = Assembly.GetExecutingAssembly()
                                  .GetExportedTypes()
                                  .Where(t => typeof (IValueParser).IsAssignableFrom(t))
                                  .Where(t => !t.IsInterface)
                                  .Where(t => !t.IsAbstract)
                                  .Select(t => (IValueParser) Activator.CreateInstance(t))
                                  .OrderBy(vp => vp.SortOrder)
                                  .ToArray();
            return parsers;
        }

        public object ParseSettingValue(Type settingValueType, string settingValueString)
        {
            var underlyingNonGenericType = Nullable.GetUnderlyingType(settingValueType);
            var isNullableType = underlyingNonGenericType != null;

            if (isNullableType)
            {
                if (string.IsNullOrEmpty(settingValueString)) return null;
                settingValueType = underlyingNonGenericType;
            }

            var parser = _valueParsers.First(p => p.CanParse(settingValueType));
            var settingValue = parser.Parse(settingValueType, settingValueString);
            return settingValue;
        }
    }
}