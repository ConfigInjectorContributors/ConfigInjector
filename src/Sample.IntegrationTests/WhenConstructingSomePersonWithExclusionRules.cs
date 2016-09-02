using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConfigInjector;
using ConfigInjector.Configuration;
using ConfigInjector.Exceptions;
using ConfigInjector.Infrastructure.Logging;
using NUnit.Framework;
using Sample.IntegrationTests.ConfigurationSettings;
using Shouldly;

namespace Sample.IntegrationTests
{
    [TestFixture]
    public class WhenConstructingSomePersonWithExclusionRules
    {
        private SomePersonSetting _somePersonSetting;

        [SetUp]
        public void SetUp()
        {
            var configurationSettings = new List<IConfigurationSetting>();

            ConfigurationConfigurator.RegisterConfigurationSettings()
                                     .FromAssemblies(Assembly.GetExecutingAssembly())
                                     .RegisterWithContainer(configurationSettings.Add)
                                     .WithLogger(new ConsoleLogger())
                                     .AllowConfigurationEntriesThatDoNotHaveSettingsClasses(false)
                                     .WithCustomValueParsers(new PersonNameValueParser())
                                     .ExcludeSettings(k => k.StartsWith("Ignore"))
                                     .DoYourThing();

            _somePersonSetting = configurationSettings.OfType<SomePersonSetting>().Single();
        }

        [Test]
        public void ThePersonsFirstNameShouldBeRandom()
        {
            _somePersonSetting.Value.FirstName.ShouldBe("Random");
        }

    }
}