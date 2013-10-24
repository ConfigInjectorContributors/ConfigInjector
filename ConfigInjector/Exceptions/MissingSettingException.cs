using System;
using ConfigInjector.Extensions;

namespace ConfigInjector.Exceptions
{
    public class MissingSettingException : ConfigurationException
    {
        public MissingSettingException(Type settingType) : base("Setting {0} was not found".FormatWith(settingType.Name))
        {
        }
    }
}