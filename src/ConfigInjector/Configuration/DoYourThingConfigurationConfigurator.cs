using System;
using System.Collections.Generic;
using System.Reflection;
using ConfigInjector.Exceptions;
using ConfigInjector.Infrastructure;
using ConfigInjector.Infrastructure.Logging;
using ConfigInjector.Infrastructure.SettingsConventions;
using ConfigInjector.Infrastructure.SettingsOverriders;
using ConfigInjector.Infrastructure.SettingsReaders;
using ConfigInjector.Infrastructure.TypeProviders;
using ConfigInjector.Infrastructure.ValueParsers;

namespace ConfigInjector.Configuration
{
    public class DoYourThingConfigurationConfigurator
    {
        private readonly ITypeProvider _typeProvider;
        private readonly Action<IConfigurationSetting> _registerAsSingleton;

        private bool _allowConfigurationEntriesThatDoNotHaveSettingsClasses;
        private readonly List<IValueParser> _customValueParsers = new List<IValueParser>();
        private ISettingsReader _settingsReader;
        private ISettingsOverrider _settingsOverrider = new NoOpSettingsOverrider();
        private IConfigInjectorLogger _logger = new NullLogger();

        private readonly List<ISettingKeyConvention> _settingKeyConventions = new List<ISettingKeyConvention>();
        private readonly List<string> _excludedKeys = new List<string>();

        internal DoYourThingConfigurationConfigurator(ITypeProvider typeProvider, Action<IConfigurationSetting> registerAsSingleton)
        {
            _typeProvider = typeProvider;
            _registerAsSingleton = registerAsSingleton;

            _settingKeyConventions.AddRange(SettingKeyConventions.BuiltInConventions);
        }

        /// <summary>
        /// If set to false (default), ConfigInjector will blow up when there are settings in the [web|app].config file that
        /// do not have corresponding setting types in your application.
        /// </summary>
        public DoYourThingConfigurationConfigurator AllowConfigurationEntriesThatDoNotHaveSettingsClasses(bool allow)
        {
            _allowConfigurationEntriesThatDoNotHaveSettingsClasses = allow;
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

        public DoYourThingConfigurationConfigurator ExcludeSettingKeys(params string[] settingKeys)
        {
            _excludedKeys.AddRange(settingKeys);
            return this;
        }

        /// <summary>
        /// This allows you to substitute your own application settings reader. A good use case for this is in having a unit/convention
        /// test suite that opens your application's app.config file (rather than the test project's one) and asserts that all configuration
        /// settings are present and accounted for.
        /// </summary>
        public DoYourThingConfigurationConfigurator WithAppSettingsReader(ISettingsReader settingsReader)
        {
            _settingsReader = settingsReader;
            return this;
        }

        public DoYourThingConfigurationConfigurator WithAppSettingsOverrider(ISettingsOverrider settingsOverrider)
        {
            _settingsOverrider = settingsOverrider;
            return this;
        }

        public DoYourThingConfigurationConfigurator WithLogger(IConfigInjectorLogger logger)
        {
            _logger = logger;
            return this;
        }

        public void DoYourThing()
        {
            if (_typeProvider == null) throw new ConfigurationException("You must specify a type provider used to scan for configuration settings.");
            if (_registerAsSingleton == null) throw new ConfigurationException("You must provide a registration action.");

            var settingsReader = _settingsReader ?? new AppSettingsReader(_excludedKeys.ToArray());
            var settingsOverrider = _settingsOverrider ?? new NoOpSettingsOverrider();
            var settingValueConverter = new SettingValueConverter(_customValueParsers.ToArray());

            var appConfigConfigurationProvider = new SettingsRegistrationService(_logger, _typeProvider, _settingKeyConventions.ToArray(), settingsReader, settingsOverrider, settingValueConverter, _allowConfigurationEntriesThatDoNotHaveSettingsClasses, _registerAsSingleton);
            appConfigConfigurationProvider.RegisterConfigurationSettings();
        }
    }

    internal class NoOpSettingsOverrider : ISettingsOverrider
    {
        public bool TryFindOverrideFor(string key, out string value)
        {
            value = null;
            return false;
        }
    }
}