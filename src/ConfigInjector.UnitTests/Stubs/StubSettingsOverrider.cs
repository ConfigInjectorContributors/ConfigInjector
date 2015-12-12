using System.Collections.Generic;
using ConfigInjector.Infrastructure.SettingsOverriders;

namespace ConfigInjector.UnitTests.Stubs
{
    internal class StubSettingsOverrider : ISettingsOverrider
    {
        private readonly Dictionary<string, string> _settingOverrides;

        public StubSettingsOverrider(Dictionary<string, string> settingOverrides)
        {
            _settingOverrides = settingOverrides;
        }

        public bool TryFindOverrideFor(string key, out string value)
        {
            return _settingOverrides.TryGetValue(key, out value);
        }
    }
}