using System;
using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Infrastructure;
using ConfigInjector.Infrastructure.Logging;

namespace ConfigInjector.Configuration
{
    public interface ISettingsLoaderBuilder
    {
        ISettingsLoader Build();
    }

    public class ConfigurationConfigurator
    {
        public IConfigInjectorLogger Logger { get; private set; } = new NullLogger();
        private ISettingsLoaderBuilder _settingsLoaderBuilder;
        private readonly List<Type> _settingsOverriderTypes = new List<Type>();

        private ConfigurationConfigurator()
        {
        }

        public static ConfigurationConfigurator RegisterConfigurationSettings()
        {
            return new ConfigurationConfigurator();
        }

        public ConfigurationConfigurator WithSettingsLoaderBuilder(ISettingsLoaderBuilder settingsLoaderBuilder)
        {
            _settingsLoaderBuilder = settingsLoaderBuilder;
            return this;
        }

        public ConfigurationConfigurator WithOverridesFrom<T>()
        {
            _settingsOverriderTypes.Add(typeof(T));
            return this;
        }

        public ConfigurationConfigurator WithLogger(IConfigInjectorLogger logger)
        {
            Logger = logger;
            return this;
        }

        public ConfigInjectorInstance DoYourThing()
        {
            var settingsLoader = _settingsLoaderBuilder.Build();
            var settings = settingsLoader.LoadConfigurationSettings().ToDictionary(s => s.GetType(), s => s);
            var instance = new ConfigInjectorInstance(settings, _settingsOverriderTypes.ToArray());
            return instance;
        }
    }
}