using System;
using System.IO;
using ConfigInjector.Configuration;
using ConfigInjector.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace ConfigInjector.Sources.DotNet
{
    public class DotNetSettingsLoaderBuilder<TConfigurationRoot> : ISettingsLoaderBuilder
        where TConfigurationRoot : IConfigurationSetting
    {
        private readonly ConfigurationConfigurator _configurator;
        private readonly Func<IConfigurationBuilder, IConfigurationBuilder> _configs;

        public DotNetSettingsLoaderBuilder(ConfigurationConfigurator configurator)
        {
            _configurator = configurator;
            _configs = c => c.SetBasePath(Directory.GetCurrentDirectory())
                             .AddJsonFile("appsettings.json");
        }

        public DotNetSettingsLoaderBuilder(ConfigurationConfigurator configurator, Func<IConfigurationBuilder, IConfigurationBuilder> configs)
        {
            _configurator = configurator;
            _configs = configs;
        }

        public ISettingsLoader Build()
        {
            return new DotNetSettingsLoader<TConfigurationRoot>(_configurator, _configs);
        }
    }
}