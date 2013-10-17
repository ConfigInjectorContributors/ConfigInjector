using System.Configuration;

namespace ConfigInjector
{
    internal class AppSettingsReader
    {
        public static string ReadValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}