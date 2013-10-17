using System;

namespace ConfigInjector.ValueParsers
{
    public class TimeSpanParser : IValueParser
    {
        public int SortOrder
        {
            get { return 100; }
        }

        public bool CanParse(Type settingValueType)
        {
            return typeof (TimeSpan).IsAssignableFrom(settingValueType);
        }

        public object Parse(Type settingValueType, string settingValueString)
        {
            var result = TimeSpan.Parse(settingValueString);
            return result;
        }
    }
}