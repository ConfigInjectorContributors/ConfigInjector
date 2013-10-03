using System;
using System.Configuration;
using ConfigInjector.Extensions;

namespace ConfigInjector
{
    internal class AppConfigConfigurationProvider
    {
        public IConfigurationSetting GetConfigSettingFor(Type type)
        {
            var settingKey = type.Name;
            var settingValueString = ConfigurationManager.AppSettings[settingKey];

            if (settingValueString == null) throw new InvalidOperationException("Setting {0} was not found in [web|app].config".FormatWith(settingKey));

            var settingType = type.GetProperty("Value").PropertyType;
            var settingValue = (dynamic)Convert.ChangeType(settingValueString, settingType);

            var setting = Activator.CreateInstance(type);
            ((dynamic)setting).Value = settingValue;

            return (IConfigurationSetting)setting;
        }
    }
}