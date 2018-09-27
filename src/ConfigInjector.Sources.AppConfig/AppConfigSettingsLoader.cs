using System;
using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Exceptions;
using ConfigInjector.Extensions;
using ConfigInjector.Infrastructure;
using ConfigInjector.Infrastructure.Logging;
using ConfigInjector.Infrastructure.TypeProviders;
using ConfigInjector.Sources.AppConfig.Exceptions;
using ConfigInjector.Sources.AppConfig.SettingsConventions;
using ConfigInjector.Sources.AppConfig.SettingsReaders;

namespace ConfigInjector.Sources.AppConfig
{
    internal class AppConfigSettingsLoader : ISettingsLoader
    {
        private readonly bool _allowEntriesInWebConfigThatDoNotHaveSettingsClasses;
        private readonly IConfigInjectorLogger _logger;
        private readonly ISettingKeyConvention[] _settingKeyConventions;
        private readonly ISettingsReader _settingsReader;
        private readonly SettingValueConverter _settingValueConverter;
        private readonly ITypeProvider _typeProvider;

        private IConfigurationSetting[] _stronglyTypedSettings;

        public AppConfigSettingsLoader(IConfigInjectorLogger logger,
                                       ITypeProvider typeProvider,
                                       ISettingKeyConvention[] settingKeyConventions,
                                       ISettingsReader settingsReader,
                                       SettingValueConverter settingValueConverter,
                                       bool allowEntriesInWebConfigThatDoNotHaveSettingsClasses)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _typeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));
            _settingKeyConventions = settingKeyConventions ?? throw new ArgumentNullException(nameof(settingKeyConventions));
            _settingsReader = settingsReader ?? throw new ArgumentNullException(nameof(settingsReader));
            _settingValueConverter = settingValueConverter ?? throw new ArgumentNullException(nameof(settingValueConverter));
            _allowEntriesInWebConfigThatDoNotHaveSettingsClasses = allowEntriesInWebConfigThatDoNotHaveSettingsClasses;
        }

        public IConfigurationSetting[] LoadConfigurationSettings()
        {
            _stronglyTypedSettings = ConstructConfigurationSettings();

            if (!_allowEntriesInWebConfigThatDoNotHaveSettingsClasses)
            {
                AssertThatNoAdditionalSettingsExist();
            }

            return _stronglyTypedSettings;
        }

        private IConfigurationSetting[] ConstructConfigurationSettings()
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
            var settingObject = ConstructSettingObject(type, setting.Value);
            _logger.Debug("Setting for type {0} loaded from settings provider (key: {1}; value: {2})", type.Name, setting.Key, settingObject.ToString());
            return settingObject;
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
                _logger.Warn("The current settings reader does not support the enumeration of settings keys. We can't confirm that there are no extraneous settings.");
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