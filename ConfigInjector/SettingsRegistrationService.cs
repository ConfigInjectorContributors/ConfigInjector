using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using ConfigInjector.Extensions;
using ConfigInjector.SettingsReaders;

namespace ConfigInjector
{
    /// <summary>
    ///     Stateful service for settings registration.
    /// </summary>
    internal class SettingsRegistrationService
    {
        private readonly Assembly[] _assemblies;
        private readonly Action<IConfigurationSetting> _registerAsSingleton;
        private readonly bool _allowEntriesInWebConfigThatDoNotHaveSettingsClasses;
        private readonly SettingValueConverter _settingValueConverter;
        private readonly List<ISettingsReader> _settingsReaders;

        private IConfigurationSetting[] _stronglyTypedSettings;

        public SettingsRegistrationService(Assembly[] assemblies,
                                           Action<IConfigurationSetting> registerAsSingleton,
                                           bool allowEntriesInWebConfigThatDoNotHaveSettingsClasses,
                                           SettingValueConverter settingValueConverter,
                                           List<ISettingsReader> settingsReaders)
        {
            _assemblies = assemblies;
            _registerAsSingleton = registerAsSingleton;
            _allowEntriesInWebConfigThatDoNotHaveSettingsClasses = allowEntriesInWebConfigThatDoNotHaveSettingsClasses;
            _settingValueConverter = settingValueConverter;
            _settingsReaders = settingsReaders;
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
            var configurationSettings = _assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => !t.IsInterface)
                .Where(t => !t.IsAbstract)
                .Where(t => typeof (IConfigurationSetting).IsAssignableFrom(t))
                .Select(GetConfigSettingFor)
                .ToArray();

            return configurationSettings;
        }

        private IConfigurationSetting GetConfigSettingFor(Type type)
        {
            var settingKey = type.Name;
            var settingValueString = _settingsReaders.Select(r => r.ReadValue(settingKey)).FirstOrDefault(v => v != null);

            if (settingValueString == null) throw new InvalidOperationException("Setting {0} was not found".FormatWith(settingKey));

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
            var extraneousWebConfigEntries = ConfigurationManager.AppSettings.AllKeys
                                                                 .Where(NoStronglyTypedSettingExistsFor)
                                                                 .ToArray();

            if (extraneousWebConfigEntries.None()) return;

            var messages = extraneousWebConfigEntries.Select(k => "Setting {0} appears in [web|app].config but has no corresponding ConfigurationSetting type.".FormatWith(k));
            var message = string.Join(Environment.NewLine, messages);
            throw new ConfigurationException(message);
        }

        private bool NoStronglyTypedSettingExistsFor(string key)
        {
            return _stronglyTypedSettings
                .Where(s => s.GetType().Name == key)
                .None();
        }
    }
}