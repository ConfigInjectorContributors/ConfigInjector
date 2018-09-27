using System;
using ConfigInjector.Configuration;
using ConfigInjector.Infrastructure;
using ConfigInjector.Sources.Object;
using Microsoft.Extensions.Configuration;

namespace ConfigInjector.Sources.DotNet
{
    public class DotNetSettingsLoader<TAppSettingsRoot> : ISettingsLoader
        where TAppSettingsRoot : IConfigurationSetting
    {
        private readonly ConfigurationConfigurator _configurator;
        private readonly Func<IConfigurationBuilder, IConfigurationBuilder> _configs;

        public DotNetSettingsLoader(ConfigurationConfigurator configurator, Func<IConfigurationBuilder, IConfigurationBuilder> configs)
        {
            _configurator = configurator;
            _configs = configs;
        }

        public IConfigurationSetting[] LoadConfigurationSettings()
        {
            var appSettingsRoot = LoadAppSettingsFromDotNetConfigurationProvider();
            var settings = ExtractConfigurationSettings(appSettingsRoot);
            return settings;
        }

        private TAppSettingsRoot LoadAppSettingsFromDotNetConfigurationProvider()
        {
            var builder = new ConfigurationBuilder();
            _configs(builder);
            var configurationRoot = builder.Build();
            var appSettingsRoot = configurationRoot.Get<TAppSettingsRoot>();
            return appSettingsRoot;
        }

        private IConfigurationSetting[] ExtractConfigurationSettings(TAppSettingsRoot appSettingsRoot)
        {
            var settingsLoaderBuilder = new ObjectSettingsLoaderBuilder(_configurator).WithConfigurationRoot(appSettingsRoot);
            var settingsLoader = settingsLoaderBuilder.Build();
            var settings = settingsLoader.LoadConfigurationSettings();
            return settings;
        }
    }
}