using System.ComponentModel.Design;
using System.Configuration;

namespace ConfigInjector.SettingsReaders
{
    public class AppSettingsReader : ISettingsReader
    {
        public string ReadValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}