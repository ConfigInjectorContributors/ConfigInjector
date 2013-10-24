using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Exceptions;
using ConfigInjector.SettingsConventions;
using NUnit.Framework;

namespace ConfigInjector.UnitTests
{
    [TestFixture]
    internal class WhenThereAreSettingsThatExistInWebConfigButAreNotAccountedFor : TestFor<SettingsRegistrationService>
    {
        protected override SettingsRegistrationService Given()
        {
            var assemblies = new[] {typeof (WhenReadingTheValueForASettingThatDoesNotExist).Assembly};

            var settingsReader = new ExtraneousSettingsReader();

            return new SettingsRegistrationService(assemblies,
                                                   setting => { },
                                                   true,
                                                   new SettingValueConverter(),
                                                   settingsReader,
                                                   SettingKeyConventions.BuiltInConventions.ToArray());
        }

        protected override void When()
        {
        }

        [Test]
        [ExpectedException(typeof (ConfigurationException))]
        public void AConfigurationExceptionShouldBeThrown()
        {
            Subject.RegisterConfigurationSettings();
        }

        private class ExtraneousSettingsReader : ISettingsReader
        {
            public string ReadValue(string key)
            {
                return null;
            }

            public IEnumerable<string> AllKeys
            {
                get
                {
                    yield return "SomeSettingThatDoesNotHaveACorrespondingType";
                    yield return "SomeOtherSettingThatDoesNotHaveACorrespondingType";
                }
            }
        }
    }
}