using System;
using System.Reflection;

namespace ConfigInjector.Configuration
{
    public class RegisterWithContainerConfigurationConfigurator
    {
        private readonly Assembly[] _assemblies;

        internal RegisterWithContainerConfigurationConfigurator(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public DoYourThingConfigurationConfigurator RegisterWithContainer(Action<IConfigurationSetting> registerAsSingleton)
        {
            return new DoYourThingConfigurationConfigurator(_assemblies, registerAsSingleton);
        }
    }
}