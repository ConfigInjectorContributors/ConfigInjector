using System;
using System.Reflection;

namespace ConfigInjector.Configuration
{
    public class DoYourThingConfigurationConfigurator
    {
        private readonly Assembly[] _assemblies;
        private readonly Action<IConfigurationSetting> _registerAsSingleton;
        private bool _allowEntriesInWebConfigThatDoNotHaveSettingsClasses;

        internal DoYourThingConfigurationConfigurator(Assembly[] assemblies, Action<IConfigurationSetting> registerAsSingleton)
        {
            _assemblies = assemblies;
            _registerAsSingleton = registerAsSingleton;
        }

        public DoYourThingConfigurationConfigurator AllowEntriesInWebConfigThatDoNotHaveSettingsClasses(bool allow)
        {
            _allowEntriesInWebConfigThatDoNotHaveSettingsClasses = allow;
            return this;
        }

        public void DoYourThing()
        {
            if (_assemblies == null) throw new ConfigurationException("You must specify the assemblies to scan for configuration settings.");
            if (_registerAsSingleton == null) throw new ConfigurationException("You must provide a registration action.");

            var appConfigConfigurationProvider = new SettingsRegistrationService(_assemblies, _registerAsSingleton, _allowEntriesInWebConfigThatDoNotHaveSettingsClasses);
            appConfigConfigurationProvider.RegisterConfigurationSettings();
        }
    }
}