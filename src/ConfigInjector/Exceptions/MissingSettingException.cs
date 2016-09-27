using System;

namespace ConfigInjector.Exceptions
{
    [Serializable]
    public class MissingSettingException : ConfigurationException
    {
        public MissingSettingException(Type settingType) : base($"Setting {settingType.Name} was not found")
        {
            Data["SettingType"] = settingType.FullName;
        }
    }
}