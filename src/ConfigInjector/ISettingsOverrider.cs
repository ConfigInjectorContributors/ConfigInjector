namespace ConfigInjector
{
    public interface ISettingsOverrider
    {
        bool TryFindOverrideFor(string key, out string value);
    }
}