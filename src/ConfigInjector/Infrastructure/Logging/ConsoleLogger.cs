using System;

namespace ConfigInjector.Infrastructure.Logging
{
    public class ConsoleLogger : IConfigInjectorLogger
    {
        private const ConsoleColor _infoColor = ConsoleColor.White;
        private const ConsoleColor _warnColor = ConsoleColor.Yellow;
        private const ConsoleColor _errorColor = ConsoleColor.Red;

        public void Debug(string template, params object[] args)
        {
            Console.ForegroundColor = _infoColor;
            Console.WriteLine(template, args);
        }

        public void Warn(string template, params object[] args)
        {
            Console.ForegroundColor = _warnColor;
            Console.WriteLine(template, args);
        }

        public void Warn(Exception exception, string template, params object[] args)
        {
            Console.ForegroundColor = _warnColor;
            Console.WriteLine(template, args);
            Console.WriteLine(exception.ToString());
        }

        public void Error(string template, params object[] args)
        {
            Console.ForegroundColor = _errorColor;
            Console.WriteLine(template, args);
        }

        public void Error(Exception exception, string template, params object[] args)
        {
            Console.ForegroundColor = _errorColor;
            Console.WriteLine(template, args);
            Console.WriteLine(exception.ToString());
        }
    }
}