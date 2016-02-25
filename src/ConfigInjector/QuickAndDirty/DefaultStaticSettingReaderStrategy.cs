using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfigInjector.Configuration;
using ConfigInjector.Infrastructure.ValueParsers;

namespace ConfigInjector.QuickAndDirty
{
    public class DefaultStaticSettingReaderStrategy : IStaticSettingReaderStrategy
    {
        public T Get<T>()
        {
            var assembliesToScan = new Assembly[0]
                .Union(AppDomain.CurrentDomain.GetAssemblies())
                .Union(new[] {typeof (T).Assembly}).ToArray();

            var settings = ReadSettings(assembliesToScan);

            return settings.OfType<T>().First();
        }

        private IEnumerable<IConfigurationSetting> ReadSettings(Assembly[] fromAssemblies)
        {
            var settings = new List<IConfigurationSetting>();

            var valueParsers = fromAssemblies
                .SelectMany(a => a.DefinedTypes)
                .Where(t => typeof (IValueParser).IsAssignableFrom(t))
                .Where(IsInstantiable)
                .Select(t => (IValueParser) Activator.CreateInstance(t))
                .ToArray();

            ConfigurationConfigurator.RegisterConfigurationSettings()
                                     .FromAssemblies(fromAssemblies)
                                     .RegisterWithContainer(settings.Add)
                                     .WithCustomValueParsers(valueParsers)
                                     .AllowConfigurationEntriesThatDoNotHaveSettingsClasses(true)
                                     .DoYourThing();

            return settings.ToArray();
        }

        private static bool IsInstantiable(Type type)
        {
            return !type.IsInterface && !type.IsAbstract && !type.ContainsGenericParameters;
        }
    }
}