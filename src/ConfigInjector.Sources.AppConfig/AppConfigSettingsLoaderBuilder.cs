using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfigInjector.Configuration;
using ConfigInjector.Infrastructure;
using ConfigInjector.Infrastructure.TypeProviders;
using ConfigInjector.Sources.AppConfig.SettingsConventions;
using ConfigInjector.Sources.AppConfig.ValueParsers;

namespace ConfigInjector.Sources.AppConfig
{
    public class AppConfigSettingsLoaderBuilder : ISettingsLoaderBuilder
    {
        private readonly ConfigurationConfigurator _configurator;
        private ITypeProvider _typeProvider;
        private readonly List<IValueParser> _customValueParsers = new List<IValueParser>();
        private readonly List<ISettingKeyConvention> _settingKeyConventions = new List<ISettingKeyConvention>();

        private readonly List<Func<string, bool>> _exclusionRules = new List<Func<string, bool>>();
        private bool _allowConfigurationEntriesThatDoNotHaveSettingsClasses;

        public AppConfigSettingsLoaderBuilder(ConfigurationConfigurator configurator)
        {
            _configurator = configurator;
        }

        public AppConfigSettingsLoaderBuilder FromAssemblies(params Assembly[] assemblies)
        {
            return FromTypeProvider(new AssemblyScanningTypeProvider(assemblies));
        }

        public AppConfigSettingsLoaderBuilder FromTypeProvider(ITypeProvider typeProvider)
        {
            _typeProvider = typeProvider;
            return this;
        }

        /// <summary>
        /// If set to false (default), ConfigInjector will blow up when there are settings in the [web|app].config file that
        /// do not have corresponding setting types in your application.
        /// </summary>
        public AppConfigSettingsLoaderBuilder AllowConfigurationEntriesThatDoNotHaveSettingsClasses(bool allow)
        {
            _allowConfigurationEntriesThatDoNotHaveSettingsClasses = allow;
            return this;
        }

        public AppConfigSettingsLoaderBuilder WithCustomValueParsers(params IValueParser[] valueParsers)
        {
            _customValueParsers.AddRange(valueParsers);
            return this;
        }

        public AppConfigSettingsLoaderBuilder WithSettingKeyConventions(params ISettingKeyConvention[] settingKeyConventions)
        {
            _settingKeyConventions.AddRange(settingKeyConventions);
            return this;
        }

        public AppConfigSettingsLoaderBuilder ExcludeSettingKeys(params string[] settingKeys)
        {
            var rules = settingKeys
                        .Select(settingKey => (Func<string, bool>) (k => settingKey == k))
                        .ToArray();
            _exclusionRules.AddRange(rules);
            return this;
        }

        public AppConfigSettingsLoaderBuilder ExcludeSettingKeys(Func<string, bool> exclusionRule)
        {
            _exclusionRules.Add(exclusionRule);
            return this;
        }

        public ISettingsLoader Build()
        {
            var settingsLoader = new AppConfigSettingsLoader(_configurator.Logger,
                                                             _typeProvider,
                                                             _settingKeyConventions.ToArray(),
                                                             new AppSettingsReader(_exclusionRules.ToArray()),
                                                             new SettingValueConverter(),
                                                             _allowConfigurationEntriesThatDoNotHaveSettingsClasses);
            return settingsLoader;
        }
    }
}