﻿using System;

namespace ConfigInjector.Infrastructure.SettingsOverriders
{
    public class EnvironmentVariableSettingsOverrider : ISettingsOverrider
    {
        private readonly string _prefix;

        public EnvironmentVariableSettingsOverrider(string prefix)
        {
            _prefix = prefix;
        }

        public bool TryFindOverrideFor(string key, out string value)
        {
            var environmentVariableKey = string.Format("{0}{1}", _prefix, key);
            value = Environment.GetEnvironmentVariable(environmentVariableKey);
            return (value != null);
        }
    }
}