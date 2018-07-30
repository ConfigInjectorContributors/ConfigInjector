using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigInjector.Infrastructure.SettingsReaders
{
    public class ObjectSettingsReader : IEnumeratingSettingsReader
    {
        private readonly object _settingsObject;
        private readonly Lazy<Dictionary<string, string>> _values;

        public ObjectSettingsReader(object settingsObject)
        {
            _settingsObject = settingsObject;
            _values = new Lazy<Dictionary<string, string>>(LoadSettings);
        }

        private Dictionary<string, string> LoadSettings()
        {
            var settings = ExtractSettings(_settingsObject, "")
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value + "");

            return settings;
        }

        private IEnumerable<KeyValuePair<string, object>> ExtractSettings(object o, string prefix)
        {
            foreach (var prop in o.GetType().GetProperties())
            {
                if (!prop.CanRead) continue;
                var value = prop.GetValue(o);
                if (value == null) continue;
                if (prop.PropertyType.IsValueType || prop.PropertyType == typeof(string))
                {
                    yield return new KeyValuePair<string, object>(prefix + prop.Name, value);
                }
                else
                {
                    foreach (var child in ExtractSettings(value, prefix + prop.Name + "_")) yield return child;
                }
            }
        }

        public string ReadValue(string key)
        {
            string value;
            return _values.Value.TryGetValue(key, out value) ? value : null;
        }

        public IEnumerable<string> AllKeys => _values.Value.Keys;
    }
}