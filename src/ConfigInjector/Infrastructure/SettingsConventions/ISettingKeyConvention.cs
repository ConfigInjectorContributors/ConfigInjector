using System;

namespace ConfigInjector.Infrastructure.SettingsConventions
{
    public interface ISettingKeyConvention
    {
        string KeyFor(Type settingType);
    }
}