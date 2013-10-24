using System.Collections.Generic;
using System.Configuration;

namespace ConfigInjector
{
    public class AppSettingsReader : ISettingsReader
    {
        public string ReadValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public IEnumerable<string> AllKeys
        {
            get { return ConfigurationManager.AppSettings.AllKeys; }
        }
    }
}