namespace ConfigInjector.Infrastructure.SettingsOverriders
{
    public class EnvironmentVariableSettingsOverrider : ISettingsOverrider
    {
        public T Override<T>(T setting) where T : IConfigurationSetting
        {
            //TODO implement me :)
            return setting;
        }
    }
}