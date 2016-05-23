namespace ConfigInjector
{
    public interface IConfigurationSetting
    {
        bool IsSensitive { get; }
    }
}