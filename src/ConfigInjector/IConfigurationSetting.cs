namespace ConfigInjector
{
    public interface IConfigurationSetting
    {
        string SanitizedValue { get; }
        bool IsSensitive { get; }
    }
}