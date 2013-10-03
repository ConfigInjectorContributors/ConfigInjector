using System;
using System.Linq;
using System.Reflection;

namespace ConfigInjector
{
    public class ConfigurationConfigurator
    {
        private Action<IConfigurationSetting> _registerAsSingleton;
        private Assembly[] _assemblies;

        private ConfigurationConfigurator()
        {
        }

        public static ConfigurationConfigurator RegisterConfigurationSettings()
        {
            return new ConfigurationConfigurator();
        }

        public ConfigurationConfigurator FromAssemblies(params Assembly[] assemblies)
        {
            _assemblies = assemblies;

            return this;
        }

        public ConfigurationConfigurator RegisterWithContainer(Action<IConfigurationSetting> registerAsSingleton)
        {
            _registerAsSingleton = registerAsSingleton;

            return this;
        }

        public void DoYourThing()
        {
            if (_assemblies == null) throw new ConfigurationException("You must specify the assemblies to scan for configuration settings.");
            if (_registerAsSingleton == null) throw new ConfigurationException("You must provide a registration action.");

            var appConfigConfigurationProvider = new AppConfigConfigurationProvider();

            var configurationSettings = _assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => !t.IsInterface)
                .Where(t => !t.IsAbstract)
                .Where(t => typeof (IConfigurationSetting).IsAssignableFrom(t))
                .Select(appConfigConfigurationProvider.GetConfigSettingFor)
                .ToArray();

            foreach (var configurationSetting in configurationSettings)
            {
                _registerAsSingleton(configurationSetting);
            }
        }
    }
}