namespace ConfigInjector.Infrastructure.SettingsOverriders
{
    public interface ISettingsOverrider
    {
        bool TryFindOverrideFor(string key, out string value);
    }
}