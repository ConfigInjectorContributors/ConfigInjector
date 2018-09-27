using System;

namespace ConfigInjector.Infrastructure.Logging
{
    public interface IConfigInjectorLogger
    {
        void Debug(string template, params object[] args);
        void Warn(string template, params object[] args);
        void Warn(Exception exception, string template, params object[] args);
        void Error(string template, params object[] args);
        void Error(Exception exception, string template, params object[] args);
    }
}