using System;

namespace ConfigInjector.ValueParsers
{
    public class EnumValueParser : IValueParser
    {
        public int SortOrder
        {
            get { return int.MaxValue - 2; }
        }

        public bool CanParse(Type settingValueType)
        {
            return settingValueType.BaseType == typeof (Enum);
        }

        public object Parse(Type settingValueType, string settingValueString)
        {
            return Enum.Parse(settingValueType, settingValueString);
        }
    }
}