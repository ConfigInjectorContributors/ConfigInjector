﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfigInjector.Configuration;
using ConfigInjector.Infrastructure.TypeProviders;
using ConfigInjector.Infrastructure.ValueParsers;

namespace ConfigInjector.QuickAndDirty
{
    public class DefaultStaticSettingReaderStrategy : IStaticSettingReaderStrategy
    {
        public T Get<T>()
        {
            var assembliesToScanForValueParsers = new Assembly[0]
                                                  .Union(AppDomain.CurrentDomain.GetAssemblies())
                                                  .Union(new[] {typeof(T).Assembly})
                                                  .Where(a => !a.IsDynamic)
                                                  .ToArray();

            var setting = ReadSetting<T>(assembliesToScanForValueParsers);

            return setting;
        }

        private static T ReadSetting<T>(Assembly[] assembliesToScanForValueParsers)
        {
            var settings = new List<IConfigurationSetting>();

            var valueParsers = assembliesToScanForValueParsers
                               .SelectMany(GetDefinedTypes)
                               .Where(t => typeof(IValueParser).IsAssignableFrom(t))
                               .Where(IsInstantiable)
                               .Select(t => (IValueParser) Activator.CreateInstance(t))
                               .ToArray();

            var singleSettingTypeProvider = new ExplicitTypeProvider(new[] {typeof(T)});
            ConfigurationConfigurator.RegisterConfigurationSettings()
                                     .FromTypeProvider(singleSettingTypeProvider)
                                     .RegisterWithContainer(settings.Add)
                                     .WithCustomValueParsers(valueParsers)
                                     .AllowConfigurationEntriesThatDoNotHaveSettingsClasses(true)
                                     .DoYourThing();

            return settings
                   .OfType<T>()
                   .Single();
        }

        private static IEnumerable<TypeInfo> GetDefinedTypes(Assembly assembly)
        {
            try
            {
                return assembly.DefinedTypes;
            }
            catch (ReflectionTypeLoadException ex)
            {
                DefaultSettingsReader.Logger.Log(ex, "Reflection type loader exception when extracting types from {AssemblyName}", assembly.FullName);
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    DefaultSettingsReader.Logger.Log(loaderException, "Loader exception: {Message}", loaderException.Message);
                }

                return Enumerable.Empty<TypeInfo>();
            }
        }

        private static bool IsInstantiable(Type type)
        {
            return !type.IsInterface && !type.IsAbstract && !type.ContainsGenericParameters;
        }
    }
}