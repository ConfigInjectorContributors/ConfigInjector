namespace ConfigInjector.QuickAndDirty
{
    public static class DefaultSettingsReader
    {
        private static IStaticSettingReaderStrategy _strategy;

        public static void SetStrategy(IStaticSettingReaderStrategy strategy)
        {
            _strategy = strategy;
        }

        static DefaultSettingsReader()
        {
            _strategy = new DefaultStaticSettingReaderStrategy();
        }

        public static T Get<T>() where T : IConfigurationSetting
        {
            return _strategy.Get<T>();
        }
    }
}