using System;
using System.Collections.Generic;
using ConfigInjector.Infrastructure.Logging;

namespace ConfigInjector.Tests.Common
{
    internal class MemoryLogger : IConfigInjectorLogger
    {
        private readonly List<LogEntry> _logEntries = new List<LogEntry>();

        public IEnumerable<LogEntry> LogEntries => _logEntries;

        public void Debug(string template, params object[] args)
        {
            _logEntries.Add(new LogEntry(template, args));
        }

        public void Warn(string template, params object[] args)
        {
            _logEntries.Add(new LogEntry(template, args));
        }

        public void Warn(Exception exception, string template, params object[] args)
        {
            _logEntries.Add(new LogEntry(exception, template, args));
        }

        public void Error(string template, params object[] args)
        {
            _logEntries.Add(new LogEntry(template, args));
        }

        public void Error(Exception exception, string template, params object[] args)
        {
            _logEntries.Add(new LogEntry(exception, template, args));
        }

        public class LogEntry
        {
            public Exception Exception { get; }
            public string Template { get; }
            public object[] Args { get; }

            public LogEntry(string template, params object[] args)
            {
                Template = template;
                Args = args;
            }

            public LogEntry(Exception exception, string template, params object[] args)
            {
                Exception = exception;
                Template = template;
                Args = args;
            }
        }
    }
}