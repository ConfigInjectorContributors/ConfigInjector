using System.Reflection;

namespace ConfigInjector.Configuration
{
    public class ConfigurationConfigurator
    {
        private ConfigurationConfigurator()
        {
        }

        public static ConfigurationConfigurator RegisterConfigurationSettings()
        {
            return new ConfigurationConfigurator();
        }

        public RegisterWithContainerConfigurationConfigurator FromAssemblies(params Assembly[] assemblies)
        {
            return new RegisterWithContainerConfigurationConfigurator(assemblies);
        }
    }
}