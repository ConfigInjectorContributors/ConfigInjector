using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Exceptions;
using ConfigInjector.Infrastructure;
using ConfigInjector.Infrastructure.Logging;
using ConfigInjector.Infrastructure.SettingsConventions;
using ConfigInjector.Infrastructure.SettingsOverriders;
using ConfigInjector.Infrastructure.SettingsReaders;
using ConfigInjector.Infrastructure.TypeProviders;
using NUnit.Framework;
using Shouldly;

namespace ConfigInjector.UnitTests.Tests.MissingSettingsTests
{
    [TestFixture]
    internal class WhenReadingTheValueForASettingThatDoesNotExist : TestFor<SettingsRegistrationService>
    {
        protected override SettingsRegistrationService Given()
        {
            var assemblies = new[] {typeof(WhenReadingTheValueForASettingThatDoesNotExist).Assembly};

            var settingsReader = new EmptySettingsReader();

            return new SettingsRegistrationService(new ConsoleLogger(),
                                                   new AssemblyScanningTypeProvider(assemblies),
                                                   SettingKeyConventions.BuiltInConventions.ToArray(),
                                                   settingsReader,
                                                   new NoOpSettingsOverrider(),
                                                   new SettingValueConverter(),
                                                   true,
                                                   setting => { }
                );
        }

        protected override void When()
        {
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

        [Test]
        public void AMissingSettingExceptionShouldBeThrown()
        {
            Should.Throw<MissingSettingException>(() => Subject.GetConfigSettingFor(typeof(SomeMissingSetting)));
        }
    }
}