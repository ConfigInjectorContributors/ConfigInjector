namespace ConfigInjector.Infrastructure.SettingsReaders
{
    public interface ISettingsReader
    {
        string ReadValue(string key);
    }
}