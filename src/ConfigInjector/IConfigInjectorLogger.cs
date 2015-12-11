namespace ConfigInjector
{
    public interface IConfigInjectorLogger
    {
        void Log(string template, params object[] args);
    }
}