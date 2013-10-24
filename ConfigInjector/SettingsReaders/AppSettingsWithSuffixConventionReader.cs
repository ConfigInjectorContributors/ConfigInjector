using System.Configuration;

namespace ConfigInjector.SettingsReaders
{
    public class AppSettingsWithSuffixConventionReader : ISettingsReader
    {
        public string ReadValue(string key)
        {
            if (!key.EndsWith("Setting"))
                return null;

            key = key.Substring(0, key.Length - "Setting".Length);
            return ConfigurationManager.AppSettings[key];
        }
    }
}