using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Infrastructure.Logging;
using ConfigInjector.Sources.AppConfig.Exceptions;
using ConfigInjector.Sources.AppConfig.SettingsConventions;
using ConfigInjector.Sources.AppConfig.SettingsReaders;
using ConfigInjector.Sources.AppConfig.Tests.Sources.AppConfig.Stubs;
using ConfigInjector.Tests.Common;
using NUnit.Framework;
using Shouldly;

namespace ConfigInjector.Sources.AppConfig.Tests.Sources.AppConfig.AmbiguousMatchTests
{
    [TestFixture]
    internal class WhenThereAreSettingsThatHaveAmbiguousMatchesInWebConfig : TestFor<AppConfigSettingsLoader>
    {
        protected override AppConfigSettingsLoader Given()
        {
            var settingsReader = new AmbiguousSettingsReader();

            return new AppConfigSettingsLoader(new ConsoleLogger(),
                                               new StubTypeProvider(typeof(SomeAmbiguousThingSetting)),
                                               SettingKeyConventions.BuiltInConventions.ToArray(),
                                               settingsReader,
                                               new SettingValueConverter(),
                                               false);
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
        public void AnAmbiguousSettingExceptionShouldBeThrown()
        {
            Should.Throw<AmbiguousSettingException>(() => Subject.LoadConfigurationSettings());
        }
    }
}