using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConfigInjector.Configuration;
using ConfigInjector.ValueParsers;
using ThirdDrawer.Extensions.CollectionExtensionMethods;
using ThirdDrawer.Extensions.TypeExtensionMethods;

namespace ConfigInjector.QuickAndDirty
{
    internal class DefaultStaticSettingReaderStrategy : IStaticSettingReaderStrategy
    {
        private readonly Lazy<IList<IConfigurationSetting>> _settings;

        public DefaultStaticSettingReaderStrategy()
        {
            _settings = new Lazy<IList<IConfigurationSetting>>(ReadSettings);
        }

        private IList<IConfigurationSetting> ReadSettings()
        {
            var settings = new List<IConfigurationSetting>();

            // ReSharper disable AssignNullToNotNullAttribute
            var assembliesInCallStack = new StackTrace()
                .GetFrames()
                .Select(f => f.GetMethod())
                .Select(m => m.DeclaringType)
                .NotNull()
                .Select(t => t.Assembly)
                .Distinct()
                .ToArray();
            // ReSharper restore AssignNullToNotNullAttribute

            var valueParsers = assembliesInCallStack
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => typeof(IValueParser).IsAssignableFrom(t))
                .Where(t => t.IsInstantiable())
                .Select(t => (IValueParser)Activator.CreateInstance(t))
                .ToArray();

            ConfigurationConfigurator.RegisterConfigurationSettings()
                                     .FromAssemblies(assembliesInCallStack)
                                     .RegisterWithContainer(settings.Add)
                                     .WithCustomValueParsers(valueParsers)
                                     .AllowConfigurationEntriesThatDoNotHaveSettingsClasses(true)
                                     .DoYourThing();

            return settings;
        }

        public T Get<T>()
        {
            return _settings.Value.OfType<T>().First();
        }
    }
}