using System;
using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Configuration;
using ConfigInjector.Infrastructure.ValueParsers;

namespace ConfigInjector.QuickAndDirty
{
    public class DefaultStaticSettingReaderStrategy : IStaticSettingReaderStrategy
    {
        private readonly Lazy<IList<IConfigurationSetting>> _settings;

        public DefaultStaticSettingReaderStrategy()
        {
            _settings = new Lazy<IList<IConfigurationSetting>>(ReadSettings);
        }

        public T Get<T>()
        {
            return _settings.Value.OfType<T>().First();
        }

        private IList<IConfigurationSetting> ReadSettings()
        {
            var settings = new List<IConfigurationSetting>();

            var assembliesInAppDomain = AppDomain.CurrentDomain.GetAssemblies();

            var valueParsers = assembliesInAppDomain
                .SelectMany(a => a.DefinedTypes)
                .Where(t => typeof (IValueParser).IsAssignableFrom(t))
                .Where(IsInstantiable)
                .Select(t => (IValueParser) Activator.CreateInstance(t))
                .ToArray();

            ConfigurationConfigurator.RegisterConfigurationSettings()
                                     .FromAssemblies(assembliesInAppDomain)
                                     .RegisterWithContainer(settings.Add)
                                     .WithCustomValueParsers(valueParsers)
                                     .AllowConfigurationEntriesThatDoNotHaveSettingsClasses(true)
                                     .DoYourThing();

            return settings;
        }

        private bool IsInstantiable(Type type)
        {
            return !type.IsInterface && !type.IsAbstract && !type.ContainsGenericParameters;
        }
    }
}