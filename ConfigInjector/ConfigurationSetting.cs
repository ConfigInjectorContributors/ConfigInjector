using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConfigInjector
{
    [DebuggerDisplay("{GetType().Name,nq}: {Value,nq}")]
    public abstract class ConfigurationSetting<T> : IConfigurationSetting
    {
        private bool _initialized;
        private T _value;

        public T Value
        {
            get { return _value; }
            set
            {
                if (_initialized) throw new InvalidOperationException("Already initialised.");

                Validate(value);

                _value = value;
                _initialized = true;
            }
        }

        protected virtual T ConvertStringValue(string settingValueString)
        {
            return (T) Convert.ChangeType(settingValueString, typeof (T));
        }

        protected virtual IEnumerable<string> ValidationErrors(T value)
        {
            yield break;
        }

        private void Validate(T value)
        {
            var validationErrors = ValidationErrors(value).ToArray();

            if (validationErrors.Any())
            {
                throw new ConfigurationSettingValidationException(validationErrors);
            }
        }

        void IConfigurationSetting.SetValueFromString(string settingValueString)
        {
            Value = ConvertStringValue(settingValueString);
        }

        public static implicit operator T(ConfigurationSetting<T> setting)
        {
            return setting.Value;
        }

        public override string ToString()
        {
            return _value as string ?? base.ToString();
        }
    }
}