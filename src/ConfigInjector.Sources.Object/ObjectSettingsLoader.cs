using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Extensions;
using ConfigInjector.Infrastructure;

namespace ConfigInjector.Sources.Object
{
    public class ObjectSettingsLoader : ISettingsLoader
    {
        private readonly object _configurationRoot;

        public ObjectSettingsLoader(object configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        public IConfigurationSetting[] LoadConfigurationSettings()
        {
            var settings = new[] {_configurationRoot}
                           .DepthFirst(Children)
                           .OfType<IConfigurationSetting>()
                           .ToArray();

            return settings;
        }

        private static IEnumerable<object> Children(object settingsObject)
        {
            return settingsObject.GetType()
                                 .GetProperties()
                                 .Where(p => !p.PropertyType.IsArray)
                                 .Where(p => !p.PropertyType.IsPrimitive)
                                 .Where(p => !p.PropertyType.IsAssignableTo<string>())
                                 .Select(p => p.GetValue(settingsObject));
        }
    }
}