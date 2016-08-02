using ConfigInjector.Infrastructure.Logging;

namespace ConfigInjector.QuickAndDirty
{
    public static class DefaultSettingsReader
    {
        private static IStaticSettingReaderStrategy _strategy;
        public static IConfigInjectorLogger Logger { get; private set; }

        public static void SetStrategy(IStaticSettingReaderStrategy strategy)
        {
            _strategy = strategy;
        }

        public static void SetLogger(IConfigInjectorLogger logger)
        {
            Logger = logger;
        }

        static DefaultSettingsReader()
        {
            _strategy = new DefaultStaticSettingReaderStrategy();
            Logger = new NullLogger();
        }

        public static T Get<T>() where T : IConfigurationSetting
        {
            return _strategy.Get<T>();
        }
    }
}