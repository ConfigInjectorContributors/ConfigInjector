using System.Collections.Generic;

namespace ConfigInjector.Infrastructure
{
    public interface ISettingsLoader
    {
        IConfigurationSetting[] LoadConfigurationSettings();
    }
}