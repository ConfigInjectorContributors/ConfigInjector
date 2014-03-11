using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ConfigInjector
{
    public class AppSettingsReader : ISettingsReader
    {
        private readonly string[] _excludedKeys;
        private readonly Lazy<Dictionary<string, string>> _settings;

        public AppSettingsReader(string[] excludedKeys)
        {
            _excludedKeys = excludedKeys;
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
                              .Where(k => !_excludedKeys.Contains(k))
                              .Select(k => new KeyValuePair<string, string>(k, appSettings[k]))
                              .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}