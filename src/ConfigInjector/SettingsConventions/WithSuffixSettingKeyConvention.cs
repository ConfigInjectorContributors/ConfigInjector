using System;

namespace ConfigInjector.SettingsConventions
{
    public class WithSuffixSettingKeyConvention : ISettingKeyConvention
    {
        private const string _suffix = "Setting";

        public string KeyFor(Type settingType)
        {
            var settingTypeName = settingType.Name;

            return settingTypeName.EndsWith(_suffix)
                       ? settingTypeName.Substring(0, settingTypeName.Length - _suffix.Length)
                       : settingTypeName;
        }
    }
}