namespace ConfigInjector.SettingsReaders
{
    public interface ISettingsReader
    {
        string ReadValue(string key);
    }
}