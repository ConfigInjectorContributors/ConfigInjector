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
        private SomePersonSetting _somePersonSetting;

        [SetUp]
        public void SetUp()
        {
            var configurationSettings = new List<IConfigurationSetting>();

            ConfigurationConfigurator.RegisterConfigurationSettings()
                                     .FromAssemblies(Assembly.GetExecutingAssembly())
                                     .RegisterWithContainer(configurationSettings.Add)
                                     .AllowEntriesInWebConfigThatDoNotHaveSettingsClasses(false)
                                     .WithCustomValueParsers(new PersonNameValueParser())
                                     .ExcludeSettingKeys("IgnoredSetting")
                                     .DoYourThing();

            _somePersonSetting = configurationSettings.OfType<SomePersonSetting>().Single();
        }

        [Test]
        public void ThePersonsFirstNameShouldBeRandom()
        {
            _somePersonSetting.Value.FirstName.ShouldBe("Random");
        }

        [Test]
        public void ThePersonsMiddleNameShouldBeFrequentFlyer()
        {
            _somePersonSetting.Value.MiddleNames.ShouldBe("Frequent Flyer");
        }

        [Test]
        public void ThePersonsLastNameShouldBeDent()
        {
            _somePersonSetting.Value.LastName.ShouldBe("Dent");
        }
    }
}