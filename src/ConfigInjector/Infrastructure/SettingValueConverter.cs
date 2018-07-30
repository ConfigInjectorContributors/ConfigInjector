using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfigInjector.Exceptions;
using ConfigInjector.Extensions;
using ConfigInjector.Infrastructure.ValueParsers;

namespace ConfigInjector.Infrastructure
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
                                  .Where(t => t.IsAssignableTo<IValueParser>())
                                  .Where(t => t.IsInstantiable())
                                  .Select(t => (IValueParser) Activator.CreateInstance(t))
                                  .OrderBy(vp => vp.SortOrder)
                                  .ToArray();
            return parsers;
        }

        public object ParseSettingValue(Type settingValueType, string settingValueString)
        {
            try
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
            catch (Exception exc)
            {
                throw new SettingParsingException("The value for this setting could not be parsed from the given text string", exc)
                      .WithData("SettingValueType", settingValueType)
                      .WithData("SettingValue", settingValueString);
            }
        }
    }
}