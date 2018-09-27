using System;

namespace ConfigInjector.Infrastructure.Logging
{
    public class NullLogger : IConfigInjectorLogger
    {
        public void Debug(string template, params object[] args)
        {
        }

        public void Warn(string template, params object[] args)
        {
        }

        public void Warn(Exception exception, string template, params object[] args)
        {
        }

        public void Error(string template, params object[] args)
        {
        }

        public void Error(Exception exception, string template, params object[] args)
        {
        }
    }
}