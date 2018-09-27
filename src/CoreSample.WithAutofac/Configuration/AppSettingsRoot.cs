using System.ComponentModel.DataAnnotations;
using ConfigInjector;

namespace CoreSample.WithAutofac.Configuration
{
    public class AppSettingsRoot : IConfigurationSetting
    {
        [Required]
        public string ApplicationName { get; set; }

        [Required]
        public SmtpSettings SmtpSettings { get; set; }

        [Required]
        public string Bork { get; set; }
    }
}