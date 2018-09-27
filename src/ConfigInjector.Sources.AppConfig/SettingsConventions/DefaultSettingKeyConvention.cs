using System;

namespace ConfigInjector.Sources.AppConfig.SettingsConventions
{
    public class DefaultSettingKeyConvention : ISettingKeyConvention
    {
        public string KeyFor(Type settingType)
        {
            return settingType.Name;
        }
    }
}