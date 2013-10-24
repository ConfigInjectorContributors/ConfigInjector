using System;

namespace ConfigInjector.SettingsConventions
{
    public interface ISettingKeyConvention
    {
        string KeyFor(Type settingType);
    }
}