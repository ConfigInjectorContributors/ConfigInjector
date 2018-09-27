using System;
using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Extensions;
using ConfigInjector.Infrastructure.SettingsOverriders;

namespace ConfigInjector
{
    public class ConfigInjectorInstance
    {
        private readonly IReadOnlyDictionary<Type, IConfigurationSetting> _settings;
        private readonly Type[] _settingsOverriderTypes;

        public ConfigInjectorInstance(IEnumerable<KeyValuePair<Type, IConfigurationSetting>> settings, Type[] settingsOverriderTypes)
        {
            _settingsOverriderTypes = settingsOverriderTypes;
            _settings = settings.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public T Get<T>(IDependencyResolver dependencyResolver) where T : IConfigurationSetting
        {
            return (T) Get(typeof(T), dependencyResolver);
        }

        public object Get(Type type, IDependencyResolver dependencyResolver)
        {
            var originalSetting = _settings[type];
            var clone = originalSetting.Clone();
            var setting = ApplyOverrides(clone, dependencyResolver);
            return setting;
        }

        private object ApplyOverrides(IConfigurationSetting setting, IDependencyResolver dependencyResolver)
        {
            foreach (var settingsOverriderType in _settingsOverriderTypes)
            {
                if (!dependencyResolver.TryResolve(settingsOverriderType, out var settingsOverrider))
                {
                    // warn
                    continue;
                }

                setting = ((ISettingsOverrider) settingsOverrider).Override(setting);
            }

            return setting;
        }
    }
}