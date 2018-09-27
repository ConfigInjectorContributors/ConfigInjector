using System;
using ConfigInjector.Configuration;

namespace ConfigInjector.Sources.Object
{
    public static class ConfigurationConfiguratorExtensions
    {
        public static ConfigurationConfigurator FromObject(this ConfigurationConfigurator configurator,
                                                           Func<ObjectSettingsLoaderBuilder, ObjectSettingsLoaderBuilder> configs)
        {
            var settingsLoaderBuilder = configs(new ObjectSettingsLoaderBuilder(configurator));

            return configurator.WithSettingsLoaderBuilder(settingsLoaderBuilder);
        }
    }
}