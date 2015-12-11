namespace ConfigInjector.Infrastructure.Logging
{
    public interface IConfigInjectorLogger
    {
        void Log(string template, params object[] args);
    }
}