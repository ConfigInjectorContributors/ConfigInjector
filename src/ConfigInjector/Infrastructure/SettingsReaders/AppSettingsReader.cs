using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ConfigInjector.Infrastructure.SettingsReaders
{
    public class AppSettingsReader : IEnumeratingSettingsReader
    {
        private readonly Lazy<Dictionary<string, string>> _settings;
        private readonly Func<string, bool> _exclusionRule;

       
        public AppSettingsReader(Func<string, bool> exclusionRule)
        {
            _exclusionRule = exclusionRule;
            _settings = new Lazy<Dictionary<string, string>>(ReadSettingsFromConfigFile);

        }

        private IDictionary<string, string> Settings
        {
            get { return _settings.Value; }
        }

        public string ReadValue(string key)
        {
            string value;
            return Settings.TryGetValue(key, out value) ? value : null;
        }

        public IEnumerable<string> AllKeys
        {
            get { return Settings.Keys; }
        }

        private Dictionary<string, string> ReadSettingsFromConfigFile()
        {
            var appSettings = ConfigurationManager.AppSettings;

            return appSettings.AllKeys
                              .Where(k => !_exclusionRule(k))
                              .Select(k => new KeyValuePair<string, string>(k, appSettings[k]))
                              .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}