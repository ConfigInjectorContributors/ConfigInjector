namespace ConfigInjector.Infrastructure.SettingsOverriders
{
    public interface ISettingsOverrider
    {
        T Override<T>(T setting) where T : IConfigurationSetting;
    }
}