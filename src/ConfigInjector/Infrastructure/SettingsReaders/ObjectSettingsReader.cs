using System.Collections.Generic;
using System.Linq;

namespace ConfigInjector.Infrastructure.SettingsReaders
{
    public class ObjectSettingsReader : IEnumeratingSettingsReader
    {
        private readonly Dictionary<string, string> _values;

        public ObjectSettingsReader(object settingsObject)
        {
            var q = from p in settingsObject.GetType().GetProperties()
                    where p.CanRead
                    let v = "" + p.GetValue(settingsObject)
                    where v != ""
                    select new {Key = p.Name, Value = v};
            _values = q.ToDictionary(x => x.Key, x => x.Value);
        }

        public string ReadValue(string key)
        {
            string value;
            return _values.TryGetValue(key, out value) ? value : null;
        }

        public IEnumerable<string> AllKeys => _values.Keys;
    }
}