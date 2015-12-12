﻿using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Exceptions;
using ConfigInjector.Infrastructure;
using ConfigInjector.Infrastructure.Logging;
using ConfigInjector.Infrastructure.SettingsConventions;
using ConfigInjector.Infrastructure.SettingsOverriders;
using ConfigInjector.Infrastructure.SettingsReaders;
using ConfigInjector.UnitTests.Stubs;
using NUnit.Framework;

namespace ConfigInjector.UnitTests.Tests.ExtraneousSettingsTests
{
    [TestFixture]
    internal class WhenThereAreSettingsThatExistInWebConfigButAreNotAccountedFor : TestFor<SettingsRegistrationService>
    {
        protected override SettingsRegistrationService Given()
        {
            var settingsReader = new ExtraneousSettingsReader();

            return new SettingsRegistrationService(new ConsoleLogger(),
                                                   new StubTypeProvider(typeof (SomeSetting)),
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

        private class ExtraneousSettingsReader : ISettingsReader
        {
            private readonly Dictionary<string, string> _settings = new[]
                                                                    {
                                                                        new KeyValuePair<string, string>(typeof (SomeSetting).Name, "DoesNotMatter"),
                                                                        new KeyValuePair<string, string>("SomeSettingThatDoesNotHaveACorrespondingType", "DoesNotMatter"),
                                                                        new KeyValuePair<string, string>("SomeOtherSettingThatDoesNotHaveACorrespondingType", "DoesNotMatter")
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

        public class SomeSetting : ConfigurationSetting<string>
        {
        }

        [Test]
        [ExpectedException(typeof (ExtraneousSettingsException))]
        public void AnExtraneousSettingsExceptionShouldBeThrown()
        {
            Subject.RegisterConfigurationSettings();
        }
    }
}