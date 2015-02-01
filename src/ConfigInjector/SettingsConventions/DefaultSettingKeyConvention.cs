using System;

namespace ConfigInjector.SettingsConventions
{
    public class DefaultSettingKeyConvention : ISettingKeyConvention
    {
        public string KeyFor(Type settingType)
        {
            return settingType.Name;
        }
    }
}