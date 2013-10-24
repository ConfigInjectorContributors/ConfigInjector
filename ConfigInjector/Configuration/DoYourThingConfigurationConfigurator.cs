using System;
using System.Collections.Generic;
using System.Reflection;
using ConfigInjector.Exceptions;
using ConfigInjector.SettingsConventions;
using ConfigInjector.ValueParsers;

namespace ConfigInjector.Configuration
{
    public class DoYourThingConfigurationConfigurator
    {
        private readonly Assembly[] _assemblies;
        private readonly Action<IConfigurationSetting> _registerAsSingleton;

        private bool _allowEntriesInWebConfigThatDoNotHaveSettingsClasses;
        private readonly List<IValueParser> _customValueParsers = new List<IValueParser>();
        private readonly ISettingsReader _settingsReader = new AppSettingsReader();

        private readonly List<ISettingKeyConvention> _settingKeyConventions = new List<ISettingKeyConvention>();

        internal DoYourThingConfigurationConfigurator(Assembly[] assemblies, Action<IConfigurationSetting> registerAsSingleton)
        {
            _assemblies = assemblies;
            _registerAsSingleton = registerAsSingleton;

            _settingKeyConventions.AddRange(SettingKeyConventions.BuiltInConventions);
        }

        public DoYourThingConfigurationConfigurator AllowEntriesInWebConfigThatDoNotHaveSettingsClasses(bool allow)
        {
            _allowEntriesInWebConfigThatDoNotHaveSettingsClasses = allow;
            return this;
        }

        public DoYourThingConfigurationConfigurator WithCustomValueParsers(params IValueParser[] valueParsers)
        {
            _customValueParsers.AddRange(valueParsers);
            return this;
        }

        public DoYourThingConfigurationConfigurator WithSettingKeyConventions(params ISettingKeyConvention[] settingKeyConventions)
        {
            _settingKeyConventions.AddRange(settingKeyConventions);
            return this;
        }

        public void DoYourThing()
        {
            if (_assemblies == null) throw new ConfigurationException("You must specify the assemblies to scan for configuration settings.");
            if (_registerAsSingleton == null) throw new ConfigurationException("You must provide a registration action.");

            var settingValueConverter = new SettingValueConverter(_customValueParsers.ToArray());

            var appConfigConfigurationProvider = new SettingsRegistrationService(_assemblies,
                                                                                 _registerAsSingleton,
                                                                                 _allowEntriesInWebConfigThatDoNotHaveSettingsClasses,
                                                                                 settingValueConverter,
                                                                                 _settingsReader,
                                                                                 _settingKeyConventions.ToArray());
            appConfigConfigurationProvider.RegisterConfigurationSettings();
        }
    }
}