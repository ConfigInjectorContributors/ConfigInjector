using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Exceptions;
using ConfigInjector.Infrastructure;
using ConfigInjector.Infrastructure.Logging;
using ConfigInjector.Infrastructure.SettingsConventions;
using ConfigInjector.Infrastructure.SettingsOverriders;
using ConfigInjector.Infrastructure.SettingsReaders;
using ConfigInjector.UnitTests.Stubs;
using NUnit.Framework;

namespace ConfigInjector.UnitTests.Tests.AmbiguousMatchTests
{
    [TestFixture]
    internal class WhenThereAreSettingsThatHaveAmbiguousMatchesInWebConfig : TestFor<SettingsRegistrationService>
    {
        protected override SettingsRegistrationService Given()
        {
            var settingsReader = new AmbiguousSettingsReader();

            return new SettingsRegistrationService(new ConsoleLogger(),
                                                   new StubTypeProvider(typeof (SomeAmbiguousThingSetting)),
                                                   SettingKeyConventions.BuiltInConventions.ToArray(),
                                                   settingsReader,
                                                   new NoOpSettingsOverrider(),
                                                   new SettingValueConverter(),
                                                   false,
                                                   setting => { }
                );
        }

        protected override void When()
        {
        }

        private class AmbiguousSettingsReader : ISettingsReader
        {
            private readonly Dictionary<string, string> _settings = new[]
                                                                    {
                                                                        new KeyValuePair<string, string>("SomeAmbiguousThing", "foo"),
                                                                        new KeyValuePair<string, string>("SomeAmbiguousThingSetting", "bar")
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

        public class SomeAmbiguousThingSetting : ConfigurationSetting<string>
        {
        }

        [Test]
        [ExpectedException(typeof (AmbiguousSettingException))]
        public void AnAmbiguousSettingExceptionShouldBeThrown()
        {
            Subject.RegisterConfigurationSettings();
        }
    }
}