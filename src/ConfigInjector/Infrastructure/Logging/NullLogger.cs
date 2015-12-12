namespace ConfigInjector.Infrastructure.Logging
{
    public class NullLogger : IConfigInjectorLogger
    {
        public void Log(string template, params object[] args)
        {
        }
    }
}