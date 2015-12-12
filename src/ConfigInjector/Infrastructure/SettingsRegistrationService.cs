using System;
using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Exceptions;
using ConfigInjector.Extensions;
using ConfigInjector.Infrastructure.Logging;
using ConfigInjector.Infrastructure.SettingsConventions;
using ConfigInjector.Infrastructure.SettingsOverriders;
using ConfigInjector.Infrastructure.SettingsReaders;
using ConfigInjector.Infrastructure.TypeProviders;

namespace ConfigInjector.Infrastructure
{
    /// <summary>
    ///     Stateful service for settings registration.
    /// </summary>
    internal class SettingsRegistrationService
    {
        private readonly bool _allowEntriesInWebConfigThatDoNotHaveSettingsClasses;
        private readonly IConfigInjectorLogger _logger;
        private readonly Action<IConfigurationSetting> _registerAsSingleton;
        private readonly ISettingKeyConvention[] _settingKeyConventions;
        private readonly ISettingsOverrider _settingsOverrider;
        private readonly ISettingsReader _settingsReader;
        private readonly SettingValueConverter _settingValueConverter;
        private readonly ITypeProvider _typeProvider;

        private IConfigurationSetting[] _stronglyTypedSettings;

        public SettingsRegistrationService(IConfigInjectorLogger logger,
                                           ITypeProvider typeProvider,
                                           ISettingKeyConvention[] settingKeyConventions,
                                           ISettingsReader settingsReader,
                                           ISettingsOverrider settingsOverrider,
                                           SettingValueConverter settingValueConverter,
                                           bool allowEntriesInWebConfigThatDoNotHaveSettingsClasses,
                                           Action<IConfigurationSetting> registerAsSingleton)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (typeProvider == null) throw new ArgumentNullException("typeProvider");
            if (settingKeyConventions == null) throw new ArgumentNullException("settingKeyConventions");
            if (settingsReader == null) throw new ArgumentNullException("settingsReader");
            if (settingsOverrider == null) throw new ArgumentNullException("settingsOverrider");
            if (settingValueConverter == null) throw new ArgumentNullException("settingValueConverter");
            if (registerAsSingleton == null) throw new ArgumentNullException("registerAsSingleton");

            _logger = logger;
            _typeProvider = typeProvider;
            _settingKeyConventions = settingKeyConventions;
            _settingsReader = settingsReader;
            _settingsOverrider = settingsOverrider;
            _settingValueConverter = settingValueConverter;
            _allowEntriesInWebConfigThatDoNotHaveSettingsClasses = allowEntriesInWebConfigThatDoNotHaveSettingsClasses;
            _registerAsSingleton = registerAsSingleton;
        }

        public void RegisterConfigurationSettings()
        {
            _stronglyTypedSettings = LoadConfigurationSettings();

            if (!_allowEntriesInWebConfigThatDoNotHaveSettingsClasses)
            {
                AssertThatNoAdditionalSettingsExist();
            }

            foreach (var configurationSetting in _stronglyTypedSettings)
            {
                _registerAsSingleton(configurationSetting);
            }
        }

        private IConfigurationSetting[] LoadConfigurationSettings()
        {
            var configurationSettings = _typeProvider.Get()
                                                     .Where(t => t.IsAssignableTo<IConfigurationSetting>())
                                                     .Where(t => t.IsInstantiable())
                                                     .Select(GetConfigSettingFor)
                                                     .ToArray();

            return configurationSettings;
        }

        internal IConfigurationSetting GetConfigSettingFor(Type type)
        {
            var potentialMatches = GetPossibleKeysFor(type)
                .ToDictionary(k => k, k => _settingsReader.ReadValue(k))
                .Where(kvp => kvp.Value != null)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var potentialMatchCount = potentialMatches.Count();
            if (potentialMatchCount == 0) throw new MissingSettingException(type);
            if (potentialMatchCount > 1) throw new AmbiguousSettingException(type, potentialMatches);

            var setting = potentialMatches.Single();
            _logger.Log("Setting for type {0} loaded from settings provider (key: {1}; value: {2})", type, setting.Key, setting.Value);

            string overriddenValue;
            if (_settingsOverrider.TryFindOverrideFor(setting.Key, out overriddenValue))
            {
                _logger.Log("Setting for type {0} overridden (key: {1}; value: {2})", type, setting.Key, overriddenValue);
                return ConstructSettingObject(type, overriddenValue);
            }

            return ConstructSettingObject(type, setting.Value);
        }

        private IConfigurationSetting ConstructSettingObject(Type type, string settingValueString)
        {
            var settingType = type.GetProperty("Value").PropertyType;
            dynamic settingValue = _settingValueConverter.ParseSettingValue(settingType, settingValueString);

            var setting = (IConfigurationSetting) Activator.CreateInstance(type);
            ((dynamic) setting).Value = settingValue;

            return setting;
        }

        private void AssertThatNoAdditionalSettingsExist()
        {
            var settingsScanner = _settingsReader as IEnumeratingSettingsReader;
            if (settingsScanner == null)
            {
                _logger.Log("WARNING: The current settings reader does not support the enumeration of settings keys. We can't confirm that there are no extraneous settings.");
                return;
            }

            var extraneousWebConfigEntries = settingsScanner.AllKeys
                                                            .Where(s => !StronglyTypedSettingExistsFor(s))
                                                            .ToArray();

            if (!extraneousWebConfigEntries.Any()) return;

            throw new ExtraneousSettingsException(extraneousWebConfigEntries);
        }

        private IEnumerable<string> GetPossibleKeysFor(Type type)
        {
            return _settingKeyConventions
                .Select(sc => sc.KeyFor(type))
                .Where(k => k != null)
                .Distinct();
        }

        private bool StronglyTypedSettingExistsFor(string key)
        {
            var possibleKeysForType = _stronglyTypedSettings.SelectMany(t => GetPossibleKeysFor(t.GetType()))
                                                            .ToArray();
            return possibleKeysForType
                .Where(k => k == key)
                .Any();
        }
    }
}