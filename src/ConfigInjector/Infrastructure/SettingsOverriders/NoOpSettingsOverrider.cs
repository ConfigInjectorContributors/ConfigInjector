﻿namespace ConfigInjector.Infrastructure.SettingsOverriders
{
    internal class NoOpSettingsOverrider : ISettingsOverrider
    {
        public bool TryFindOverrideFor(string key, out string value)
        {
            value = null;
            return false;
        }
    }
}