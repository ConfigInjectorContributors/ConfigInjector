using System;

namespace ConfigInjector.Infrastructure.Logging
{
    public interface IConfigInjectorLogger
    {
        void Log(string template, params object[] args);
        void Log(Exception exception, string template, params object[] args);
    }
}