namespace ConfigInjector.Infrastructure.Logging
{
    internal class NullLogger : IConfigInjectorLogger
    {
        public void Log(string template, params object[] args)
        {
        }
    }
}