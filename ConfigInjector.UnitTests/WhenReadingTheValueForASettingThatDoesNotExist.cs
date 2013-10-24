using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Exceptions;
using ConfigInjector.SettingsConventions;
using NUnit.Framework;

namespace ConfigInjector.UnitTests
{
    [TestFixture]
    internal class WhenReadingTheValueForASettingThatDoesNotExist : TestFor<SettingsRegistrationService>
    {
        protected override SettingsRegistrationService Given()
        {
            var assemblies = new[] {typeof (WhenReadingTheValueForASettingThatDoesNotExist).Assembly};

            var settingsReader = new EmptySettingsReader();

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
        [ExpectedException(typeof (MissingSettingException))]
        public void AMissingSettingExceptionShouldBeThrown()
        {
            Subject.GetConfigSettingFor(typeof (SomeMissingSetting));
        }

        private class SomeMissingSetting : ConfigurationSetting<string>
        {
        }

        private class EmptySettingsReader : ISettingsReader
        {
            public string ReadValue(string key)
            {
                return null;
            }

            public IEnumerable<string> AllKeys
            {
                get { yield break; }
            }
        }
    }
}