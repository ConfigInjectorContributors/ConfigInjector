using System;

namespace ConfigInjector
{
    internal class SettingValueConverter
    {
        public static object ParseSettingValue(Type settingValueType, string settingValueString)
        {
            var settingValue = (settingValueType.BaseType == typeof (Enum))
                                   ? Enum.Parse(settingValueType, settingValueString)
                                   : (dynamic) Convert.ChangeType(settingValueString, settingValueType);
            return settingValue;
        }
    }
}