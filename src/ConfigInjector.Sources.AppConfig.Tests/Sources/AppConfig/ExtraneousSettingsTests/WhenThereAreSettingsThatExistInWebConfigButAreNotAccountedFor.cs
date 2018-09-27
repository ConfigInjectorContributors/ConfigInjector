using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Exceptions;
using ConfigInjector.Infrastructure.Logging;
using ConfigInjector.Sources.AppConfig.SettingsConventions;
using ConfigInjector.Sources.AppConfig.SettingsReaders;
using ConfigInjector.Sources.AppConfig.Tests.Sources.AppConfig.Stubs;
using ConfigInjector.Tests.Common;
using NUnit.Framework;
using Shouldly;

namespace ConfigInjector.Sources.AppConfig.Tests.Sources.AppConfig.ExtraneousSettingsTests
{
    [TestFixture]
    internal class WhenThereAreSettingsThatExistInWebConfigButAreNotAccountedFor : TestFor<AppConfigSettingsLoader>
    {
        protected override AppConfigSettingsLoader Given()
        {
            var settingsReader = new ExtraneousSettingsReader();

            return new AppConfigSettingsLoader(new ConsoleLogger(),
                                               new StubTypeProvider(typeof(SomeSetting)),
                                               SettingKeyConventions.BuiltInConventions.ToArray(),
                                               settingsReader,
                                               new SettingValueConverter(),
                                               false);
        }

        protected override void When()
        {
        }

        private class ExtraneousSettingsReader : IEnumeratingSettingsReader
        {
            private readonly Dictionary<string, string> _settings = new[]
                                                                    {
                                                                        new KeyValuePair<string, string>(typeof(SomeSetting).Name, "DoesNotMatter"),
                                                                        new KeyValuePair<string, string>("SomeSettingThatDoesNotHaveACorrespondingType", "DoesNotMatter"),
                                                                        new KeyValuePair<string, string>("SomeOtherSettingThatDoesNotHaveACorrespondingType", "DoesNotMatter")
                                                                    }.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            public string ReadValue(string key)
            {
                return _settings.TryGetValue(key, out var value) ? value : null;
            }

            public IEnumerable<string> AllKeys => _settings.Keys;
        }

        public class SomeSetting : ConfigurationSetting<string>
        {
        }

        [Test]
        public void AnExtraneousSettingsExceptionShouldBeThrown()
        {
            Should.Throw<ExtraneousSettingsException>(() => Subject.LoadConfigurationSettings());
        }
    }
}