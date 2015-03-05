﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfigInjector.Exceptions;
using ConfigInjector.SettingsConventions;
using ConfigInjector.TypeProviders;
using ThirdDrawer.Extensions;
using ThirdDrawer.Extensions.CollectionExtensionMethods;

namespace ConfigInjector
{
    /// <summary>
    ///     Stateful service for settings registration.
    /// </summary>
    internal class SettingsRegistrationService
    {
        private readonly ITypeProvider _typeProvider;
        private readonly Action<IConfigurationSetting> _registerAsSingleton;
        private readonly bool _allowEntriesInWebConfigThatDoNotHaveSettingsClasses;
        private readonly bool _allowSettingsClassesThatDoNotHaveEntriesInWebConfig;
        private readonly SettingValueConverter _settingValueConverter;
        private readonly ISettingsReader _settingsReader;
        private readonly ISettingKeyConvention[] _settingKeyConventions;

        private IConfigurationSetting[] _stronglyTypedSettings;

        public SettingsRegistrationService(ITypeProvider typeProvider,
                                           Action<IConfigurationSetting> registerAsSingleton,
                                           bool allowEntriesInWebConfigThatDoNotHaveSettingsClasses,
                                           bool allowSettingsClassesThatDoNotHaveEntriesInWebConfig,
                                           SettingValueConverter settingValueConverter,
                                           ISettingsReader settingsReader,
                                           ISettingKeyConvention[] settingKeyConventions)
        {
            _typeProvider = typeProvider;
            _registerAsSingleton = registerAsSingleton;
            _allowEntriesInWebConfigThatDoNotHaveSettingsClasses = allowEntriesInWebConfigThatDoNotHaveSettingsClasses;
            _allowSettingsClassesThatDoNotHaveEntriesInWebConfig = allowSettingsClassesThatDoNotHaveEntriesInWebConfig;
            _settingValueConverter = settingValueConverter;
            _settingsReader = settingsReader;
            _settingKeyConventions = settingKeyConventions;
        }

        public void RegisterConfigurationSettings()
        {
            _stronglyTypedSettings = LoadConfigurationSettings();

            if (!_allowEntriesInWebConfigThatDoNotHaveSettingsClasses && !_allowSettingsClassesThatDoNotHaveEntriesInWebConfig)
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
                .Where(t => !t.IsInterface)
                .Where(t => !t.IsAbstract)
                .Where(t => typeof (IConfigurationSetting).IsAssignableFrom(t))
                .Select(GetConfigSettingFor)
                .NotNull()
                .ToArray();

            return configurationSettings;
        }

        internal IConfigurationSetting GetConfigSettingFor(Type type)
        {
            var settingValueStrings = GetPossibleKeysFor(type)
                .Select(k => _settingsReader.ReadValue(k))
                .NotNull()
                .ToArray();

            var matchingSettingCount = settingValueStrings.Count();
            if (matchingSettingCount == 0)
            {
                if (_allowSettingsClassesThatDoNotHaveEntriesInWebConfig)
                {
                    return null;
                }

                throw new MissingSettingException(type);
            }
            if (matchingSettingCount > 1) throw new AmbiguousSettingException(type, settingValueStrings);

            var settingValueString = settingValueStrings.Single();
            return ConstructSettingObject(type, settingValueString);
        }

        private IConfigurationSetting ConstructSettingObject(Type type, string settingValueString)
        {
            var settingType = type.GetProperty("Value").PropertyType;
            var settingValue = (dynamic) _settingValueConverter.ParseSettingValue(settingType, settingValueString);

            var setting = (IConfigurationSetting) Activator.CreateInstance(type);
            ((dynamic) setting).Value = settingValue;

            return setting;
        }

        private void AssertThatNoAdditionalSettingsExist()
        {
            var extraneousWebConfigEntries = _settingsReader.AllKeys
                                                            .Where(s => !StronglyTypedSettingExistsFor(s))
                                                            .ToArray();

            if (extraneousWebConfigEntries.None()) return;

            throw new ExtraneousSettingsException(extraneousWebConfigEntries);
        }

        private IEnumerable<string> GetPossibleKeysFor(Type type)
        {
            return _settingKeyConventions
                .Select(sc => sc.KeyFor(type))
                .NotNull()
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