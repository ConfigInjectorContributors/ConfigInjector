namespace ConfigInjector.Infrastructure.SettingsOverriders
{
    public class NoOpSettingsOverrider : ISettingsOverrider
    {
        public T Override<T>(T setting) where T : IConfigurationSetting
        {
            return setting;
        }
    }
}