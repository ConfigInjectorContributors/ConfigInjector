using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfigInjector;
using ConfigInjector.Configuration;
using NUnit.Framework;
using Sample.IntegrationTests.ConfigurationSettings;
using Shouldly;

namespace Sample.IntegrationTests
{
    [TestFixture]
    public class WhenConstructingSomePerson
    {
        private SomePersonConfigurationSetting _somePersonConfigurationSetting;

        [SetUp]
        public void SetUp()
        {
            var configurationSettings = new List<IConfigurationSetting>();

            ConfigurationConfigurator.RegisterConfigurationSettings()
                                     .FromAssemblies(Assembly.GetExecutingAssembly())
                                     .RegisterWithContainer(configurationSettings.Add)
                                     .AllowEntriesInWebConfigThatDoNotHaveSettingsClasses(false)
                                     .WithCustomValueParsers(new PersonNameValueParser())
                                     .DoYourThing();

            _somePersonConfigurationSetting = configurationSettings.OfType<SomePersonConfigurationSetting>().Single();
        }

        [Test]
        public void ThePersonsFirstNameShouldBeRandom()
        {
            _somePersonConfigurationSetting.Value.FirstName.ShouldBe("Random");
        }

        [Test]
        public void ThePersonsMiddleNameShouldBeFrequentFlyer()
        {
            _somePersonConfigurationSetting.Value.MiddleNames.ShouldBe("Frequent Flyer");
        }

        [Test]
        public void ThePersonsLastNameShouldBeDent()
        {
            _somePersonConfigurationSetting.Value.LastName.ShouldBe("Dent");
        }
    }
}