using System;
using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Sources.AppConfig.SettingsReaders;

namespace ConfigInjector.Sources.AppConfig
{
    public class AppSettingsReader : IEnumeratingSettingsReader
    {
        private readonly Func<string, bool>[] _exclusionRules;
        private readonly Lazy<Dictionary<string, string>> _settings;
        private IDictionary<string, string> Settings => _settings.Value;

        public IEnumerable<string> AllKeys => Settings.Keys;

        public AppSettingsReader(Func<string, bool>[] exclusionRules)
        {
            _exclusionRules = exclusionRules;
            _settings = new Lazy<Dictionary<string, string>>(ReadSettingsFromConfigFile);
        }

        public string ReadValue(string key)
        {
            return Settings.TryGetValue(key, out var value) ? value : null;
        }

        private Dictionary<string, string> ReadSettingsFromConfigFile()
        {
#if ( NET452 )
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
#else
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
#endif
            return appSettings.AllKeys
                              .Where(k => !IsExcludedByRules(k))
                              .Select(k => new KeyValuePair<string, string>(k, appSettings[k]))
                              .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private bool IsExcludedByRules(string settingsKey)
        {
            return _exclusionRules.Any(r => r(settingsKey));
        }
    }
}