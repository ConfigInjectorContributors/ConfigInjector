using ConfigInjector;

namespace CoreSample.WithAutofac.Configuration
{
    public class SmtpSettings : IConfigurationSetting
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int RateLimitMessagesPerMinute { get; set; }
    }
}