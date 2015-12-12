using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Infrastructure;
using ConfigInjector.Infrastructure.Logging;
using ConfigInjector.Infrastructure.SettingsConventions;
using ConfigInjector.Infrastructure.SettingsOverriders;
using ConfigInjector.Infrastructure.SettingsReaders;
using ConfigInjector.UnitTests.Stubs;
using NUnit.Framework;
using Shouldly;

namespace ConfigInjector.UnitTests.Tests.ObjectSettingsReaderTests
{
    internal class WhenReadingSettingsUsingAnObjectSettingsReader : TestFor<SettingsRegistrationService>
    {
        private List<IConfigurationSetting> _results;

        protected override SettingsRegistrationService Given()
        {
            var typeProvider = new StubTypeProvider(typeof(Foo), typeof(Bar));

            var settingsObject = new
            {
                Foo = "FooValue",
                Bar = 12
            };
            var settingsReader = new ObjectSettingsReader(settingsObject);

            var settingsOverrider = new NoOpSettingsOverrider();

            return new SettingsRegistrationService(new ConsoleLogger(),
                                                   typeProvider,
                                                   SettingKeyConventions.BuiltInConventions.ToArray(),
                                                   settingsReader,
                                                   settingsOverrider,
                                                   new SettingValueConverter(),
                                                   false,
                                                   setting => _results.Add(setting)
                );
        }

        protected override void When()
        {
            _results = new List<IConfigurationSetting>();
            Subject.RegisterConfigurationSettings();
        }

        private class Foo : ConfigurationSetting<string>
        {
        }

        private class Bar : ConfigurationSetting<int>
        {
        }

        [Test]
        public void FooShouldBeFooValue()
        {
            _results
                .OfType<Foo>()
                .Single()
                .Value
                .ShouldBe("FooValue");
        }

        [Test]
        public void BarShouldBe12()
        {
            _results
                .OfType<Bar>()
                .Single()
                .Value
                .ShouldBe(12);
        }
    }
}