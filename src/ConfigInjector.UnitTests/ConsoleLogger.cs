using System;
using ConfigInjector.Infrastructure.Logging;

namespace ConfigInjector.UnitTests
{
    internal class ConsoleLogger : IConfigInjectorLogger
    {
        public void Log(string template, params object[] args)
        {
            Console.WriteLine(template, args);
        }
    }
}