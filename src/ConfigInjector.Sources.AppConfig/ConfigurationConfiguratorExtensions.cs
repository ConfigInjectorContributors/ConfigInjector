using System;
using ConfigInjector.Configuration;

namespace ConfigInjector.Sources.AppConfig
{
    public static class ConfigurationConfiguratorExtensions
    {
        public static ConfigurationConfigurator FromAppConfig(this ConfigurationConfigurator configurator, Func<AppConfigSettingsLoaderBuilder, AppConfigSettingsLoaderBuilder> configs)
        {
            var settingsLoaderBuilder = configs(new AppConfigSettingsLoaderBuilder(configurator));

            return configurator.WithSettingsLoaderBuilder(settingsLoaderBuilder);
        }
    }
}