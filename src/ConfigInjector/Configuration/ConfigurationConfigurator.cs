using System.Reflection;
using ConfigInjector.TypeProviders;

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
            return FromTypeProvider(new AssemblyScanningTypeProvider(assemblies));
        }

        public RegisterWithContainerConfigurationConfigurator FromTypeProvider(ITypeProvider typeProvider)
        {
            return new RegisterWithContainerConfigurationConfigurator(typeProvider);
        }
    }
}