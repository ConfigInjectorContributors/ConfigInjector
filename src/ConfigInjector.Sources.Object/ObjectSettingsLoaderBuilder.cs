using ConfigInjector.Configuration;
using ConfigInjector.Infrastructure;

namespace ConfigInjector.Sources.Object
{
    public class ObjectSettingsLoaderBuilder : ISettingsLoaderBuilder
    {
        private readonly ConfigurationConfigurator _configurator;
        private object _configurationRoot;

        public ObjectSettingsLoaderBuilder(ConfigurationConfigurator configurator)
        {
            _configurator = configurator;
        }

        public ObjectSettingsLoaderBuilder WithConfigurationRoot(object configurationRoot)
        {
            _configurationRoot = configurationRoot;
            return this;
        }

        public ISettingsLoader Build()
        {
            return new ObjectSettingsLoader(_configurationRoot);
        }
    }
}