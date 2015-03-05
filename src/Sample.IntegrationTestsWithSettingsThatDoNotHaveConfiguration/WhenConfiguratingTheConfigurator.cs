using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ConfigInjector;
using ConfigInjector.Configuration;
using ConfigInjector.Exceptions;
using NUnit.Framework;
using Shouldly;

namespace Sample.IntegrationTestsWithSettingsThatDoNotHaveConfiguration
{
    public class WhenConfiguratingTheConfigurator
    {
        [TestFixture]
        public class WithDefaultConfiguration
        {
            [Test]
            public void TheConfiguratorShouldBlowUp()
            {
                // Because there is a configuration setting that isn't
                // in App.Settings (SomethingThatIsNotInAppConfigSetting)

                Should.Throw<MissingSettingException>(() =>
                {
                    var configurationSettings = new List<IConfigurationSetting>();
                    
                    ConfigurationConfigurator.RegisterConfigurationSettings()
                                             .FromAssemblies(Assembly.GetExecutingAssembly())
                                             .RegisterWithContainer(configurationSettings.Add)
                                             .DoYourThing();
                });
            }
        }

        [TestFixture]
        public class AllowingConfigurationEntriesThatDoNotHaveSettingsClasses
        {
            [Test]
            public void TheConfiguratorShouldNotBlowUp()
            {
                Should.NotThrow(() =>
                {
                    var configurationSettings = new List<IConfigurationSetting>();

                    ConfigurationConfigurator.RegisterConfigurationSettings()
                                             .FromAssemblies(Assembly.GetExecutingAssembly())
                                             .RegisterWithContainer(configurationSettings.Add)
                                             .AllowSettingsClassesThatDoNotHaveConfigurationEntries(true)
                                             .DoYourThing();
                });
            }
        }
    }
}
