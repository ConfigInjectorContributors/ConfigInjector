namespace ConfigInjector.Sources.AppConfig.SettingsReaders
{
    public interface ISettingsReader
    {
        string ReadValue(string key);
    }
}