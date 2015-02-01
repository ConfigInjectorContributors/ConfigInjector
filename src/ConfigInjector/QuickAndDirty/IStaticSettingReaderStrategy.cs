namespace ConfigInjector.QuickAndDirty
{
    public interface IStaticSettingReaderStrategy
    {
        T Get<T>();
    }
}