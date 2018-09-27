using System;
using ConfigInjector.Configuration;
using Microsoft.Extensions.Configuration;

namespace ConfigInjector.Sources.DotNet
{
    public static class ConfigurationConfiguratorExtensions
    {
        public static ConfigurationConfigurator FromDotNetConfiguration<TAppSettingsRoot>(
            this ConfigurationConfigurator configurator)
            where TAppSettingsRoot : IConfigurationSetting
        {
            var settingsLoaderBuilder = new DotNetSettingsLoaderBuilder<TAppSettingsRoot>(configurator);
            return configurator.WithSettingsLoaderBuilder(settingsLoaderBuilder);
        }

        public static ConfigurationConfigurator FromDotNetConfiguration<TAppSettingsRoot>(
            this ConfigurationConfigurator configurator,
            Func<IConfigurationBuilder, IConfigurationBuilder> configs)
            where TAppSettingsRoot : IConfigurationSetting
        {
            var settingsLoaderBuilder = new DotNetSettingsLoaderBuilder<TAppSettingsRoot>(configurator, configs);
            return configurator.WithSettingsLoaderBuilder(settingsLoaderBuilder);
        }
    }
}