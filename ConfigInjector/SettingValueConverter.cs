using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfigInjector.ValueParsers;

namespace ConfigInjector
{
    internal class SettingValueConverter
    {
        private static readonly IValueParser[] _valueParsers;

        static SettingValueConverter()
        {
            _valueParsers = LoadValueParsers().ToArray();
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

        public static object ParseSettingValue(Type settingValueType, string settingValueString)
        {
            var parser = _valueParsers.First(p => p.CanParse(settingValueType));
            var settingValue = parser.Parse(settingValueType, settingValueString);
            return settingValue;
        }
    }
}