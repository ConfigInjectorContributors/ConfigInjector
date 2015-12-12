using System;

namespace ConfigInjector.Infrastructure.ValueParsers
{
    public interface IValueParser
    {
        int SortOrder { get; }
        bool CanParse(Type settingValueType);
        object Parse(Type settingValueType, string settingValueString);
    }
}