﻿using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Exceptions;
using ConfigInjector.SettingsConventions;
using ConfigInjector.TypeProviders;
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

            return new SettingsRegistrationService(new AssemblyScanningTypeProvider(assemblies),
                                                   setting => { },
                                                   false,
                                                   false,
                                                   new SettingValueConverter(),
                                                   settingsReader,
                                                   SettingKeyConventions.BuiltInConventions.ToArray());
        }

        protected override void When()
        {
        }

        [Test]
        [ExpectedException(typeof (ExtraneousSettingsException))]
        public void AnExtraneousSettingsExceptionShouldBeThrown()
        {
            Subject.RegisterConfigurationSettings();
        }

        private class ExtraneousSettingsReader : ISettingsReader
        {
            private readonly Dictionary<string, string> _settings = new[]
            {
                new KeyValuePair<string, string>(typeof (SomeAmbiguousThingSetting).Name, "DoesNotMatter"),
                new KeyValuePair<string, string>("SomeSettingThatDoesNotHaveACorrespondingType", "DoesNotMatter"),
                new KeyValuePair<string, string>("SomeOtherSettingThatDoesNotHaveACorrespondingType", "DoesNotMatter"),
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