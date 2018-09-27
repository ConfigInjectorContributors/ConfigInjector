using System;

namespace ConfigInjector.QuickAndDirty
{
    public static class DefaultSettingsReader
    {
        public static T Get<T>() where T : IConfigurationSetting
        {
            throw new NotImplementedException();
        }
    }
}