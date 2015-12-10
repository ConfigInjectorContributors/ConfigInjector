using System;

namespace ConfigInjector.Exceptions
{
    public class AmbiguousSettingException : ConfigurationException
    {
        public AmbiguousSettingException(Type settingType, string[] matchingValues)
            : base(string.Format("Setting {0} is ambiguous and has more than one match (Values are {1})", settingType.FullName, string.Join(", ", matchingValues)))
        {
        }
    }
}