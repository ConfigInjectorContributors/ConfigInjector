using System;
using System.Diagnostics;

namespace ConfigInjector
{
    [DebuggerDisplay("{GetType().Name,nq}: {Value,nq}")]
    public abstract class ConfigurationSetting<T>: IConfigurationSetting
    {
        private bool _initialized;
        private T _value;

        public T Value
        {
            get { return _value; }
            set
            {
                if (_initialized) throw new InvalidOperationException("Already initialised.");

                _value = value;
                _initialized = true;
            }
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