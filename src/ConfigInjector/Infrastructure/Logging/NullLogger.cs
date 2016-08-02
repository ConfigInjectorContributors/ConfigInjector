using System;

namespace ConfigInjector.Infrastructure.Logging
{
    public class NullLogger : IConfigInjectorLogger
    {
        public void Log(string template, params object[] args)
        {
        }

        public void Log(Exception exception, string template, params object[] args)
        {
        }
    }
}