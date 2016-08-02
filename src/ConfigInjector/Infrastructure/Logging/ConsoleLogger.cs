using System;

namespace ConfigInjector.Infrastructure.Logging
{
    public class ConsoleLogger : IConfigInjectorLogger
    {
        public void Log(string template, params object[] args)
        {
            Console.WriteLine(template, args);
        }

        public void Log(Exception exception, string template, params object[] args)
        {
            Console.WriteLine(template, args);
            Console.WriteLine(exception.ToString());
        }
    }
}