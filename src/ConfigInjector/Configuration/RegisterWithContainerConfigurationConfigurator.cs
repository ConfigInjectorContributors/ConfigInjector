using System;
using System.Reflection;
using ConfigInjector.TypeProviders;

namespace ConfigInjector.Configuration
{
    public class RegisterWithContainerConfigurationConfigurator
    {
        private readonly ITypeProvider _typeProvider;

        internal RegisterWithContainerConfigurationConfigurator(ITypeProvider typeProvider)
        {
            _typeProvider = typeProvider;
        }

        public DoYourThingConfigurationConfigurator RegisterWithContainer(Action<IConfigurationSetting> registerAsSingleton)
        {
            return new DoYourThingConfigurationConfigurator(_typeProvider, registerAsSingleton);
        }
    }
}