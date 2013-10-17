using System;

namespace ConfigInjector.ValueParsers
{
    public class UriValueParser : IValueParser
    {
        public int SortOrder
        {
            get { return 100; }
        }

        public bool CanParse(Type settingValueType)
        {
            return typeof (Uri).IsAssignableFrom(settingValueType);
        }

        public object Parse(Type settingValueType, string settingValueString)
        {
            return new Uri(settingValueString);
        }
    }
}