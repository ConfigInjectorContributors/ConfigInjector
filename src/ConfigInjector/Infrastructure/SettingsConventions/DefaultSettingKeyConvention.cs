using System;

namespace ConfigInjector.Infrastructure.SettingsConventions
{
    public class DefaultSettingKeyConvention : ISettingKeyConvention
    {
        public string KeyFor(Type settingType)
        {
            return settingType.Name;
        }
    }
}