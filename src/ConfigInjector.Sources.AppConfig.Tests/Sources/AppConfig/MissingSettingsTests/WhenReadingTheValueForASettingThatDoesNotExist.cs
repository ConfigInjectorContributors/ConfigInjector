using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Exceptions;
using ConfigInjector.Infrastructure.Logging;
using ConfigInjector.Infrastructure.TypeProviders;
using ConfigInjector.Sources.AppConfig.SettingsConventions;
using ConfigInjector.Sources.AppConfig.SettingsReaders;
using ConfigInjector.Tests.Common;
using NUnit.Framework;
using Shouldly;

namespace ConfigInjector.Sources.AppConfig.Tests.Sources.AppConfig.MissingSettingsTests
{
    [TestFixture]
    internal class WhenReadingTheValueForASettingThatDoesNotExist : TestFor<AppConfigSettingsLoader>
    {
        protected override AppConfigSettingsLoader Given()
        {
            var assemblies = new[] {typeof(WhenReadingTheValueForASettingThatDoesNotExist).Assembly};

            var settingsReader = new EmptySettingsReader();

            return new AppConfigSettingsLoader(new ConsoleLogger(),
                                               new AssemblyScanningTypeProvider(assemblies),
                                               SettingKeyConventions.BuiltInConventions.ToArray(),
                                               settingsReader,
                                               new SettingValueConverter(),
                                               true);
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