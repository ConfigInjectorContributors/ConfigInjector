using System;
using System.IO;
using ConfigInjector;
using Newtonsoft.Json;

namespace CoreSample
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public class SmtpSettings
    {
        public string Host { get; set; }
        public SmtpSettings_Port Port { get; set; }
        public int RateLimitMessagesPerMinute { get; set; }
    }

    public class ConfigurationRoot
    {
        public SmtpSettings SmtpSettings { get; set; }
    }

    public class SmtpSettings_Port : ConfigurationSetting<int>
    {
    }

   
   
}