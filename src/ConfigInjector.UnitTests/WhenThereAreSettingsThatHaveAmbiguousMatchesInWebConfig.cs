using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Exceptions;
using ConfigInjector.SettingsConventions;
using ConfigInjector.TypeProviders;
using NUnit.Framework;

namespace ConfigInjector.UnitTests
{
    [TestFixture]
    internal class WhenThereAreSettingsThatHaveAmbiguousMatchesInWebConfig : TestFor<SettingsRegistrationService>
    {
        protected override SettingsRegistrationService Given()
        {
            var assemblies = new[] {typeof (WhenReadingTheValueForASettingThatDoesNotExist).Assembly};

            var settingsReader = new AmbiguousSettingsReader();

            return new SettingsRegistrationService(new AssemblyScanningTypeProvider(assemblies), 
                                                   setting => { },
                                                   false,
                                                   new SettingValueConverter(),
                                                   settingsReader,
                                                   SettingKeyConventions.BuiltInConventions.ToArray());
        }

        protected override void When()
        {
        }

        [Test]
        [ExpectedException(typeof (AmbiguousSettingException))]
        public void AnAmbiguousSettingExceptionShouldBeThrown()
        {
            Subject.RegisterConfigurationSettings();
        }

        private class AmbiguousSettingsReader : ISettingsReader
        {
            private readonly Dictionary<string, string> _settings = new[]
            {
                new KeyValuePair<string, string>("SomeAmbiguousThing", "foo"),
                new KeyValuePair<string, string>("SomeAmbiguousThingSetting", "bar"),
            }.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            public string ReadValue(string key)
            {
                string value;
                return _settings.TryGetValue(key, out value) ? value : null;
            }

            public IEnumerable<string> AllKeys
            {
                get { return _settings.Keys; }
            }
        }
    }
}