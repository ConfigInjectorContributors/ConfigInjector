using System;
using ConfigInjector.Extensions;

namespace ConfigInjector.Exceptions
{
    public class AmbiguousSettingException : ConfigurationException
    {
        public AmbiguousSettingException(Type settingType, string[] matchingValues)
            : base("Setting {0} is ambiguous and has more than one match (Values are {1})".FormatWith(settingType.FullName, string.Join(", ", matchingValues)))
        {
        }
    }
}