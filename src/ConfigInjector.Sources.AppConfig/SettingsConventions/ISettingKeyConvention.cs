using System;

namespace ConfigInjector.Sources.AppConfig.SettingsConventions
{
    public interface ISettingKeyConvention
    {
        string KeyFor(Type settingType);
    }
}