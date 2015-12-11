namespace ConfigInjector.Configuration
{
    internal class NullLogger : IConfigInjectorLogger
    {
        public void Log(string template, params object[] args)
        {
        }
    }
}