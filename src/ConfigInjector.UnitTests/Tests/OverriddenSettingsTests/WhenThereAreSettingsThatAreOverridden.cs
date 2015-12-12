using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Infrastructure;
using ConfigInjector.Infrastructure.Logging;
using ConfigInjector.Infrastructure.SettingsConventions;
using ConfigInjector.UnitTests.Stubs;
using NUnit.Framework;
using Shouldly;

namespace ConfigInjector.UnitTests.Tests.OverriddenSettingsTests
{
    [TestFixture]
    internal class WhenThereAreSettingsThatAreOverridden : TestFor<SettingsRegistrationService>
    {
        private List<IConfigurationSetting> _results;

        protected override SettingsRegistrationService Given()
        {
            _results = new List<IConfigurationSetting>();

            var typeProvider = new StubTypeProvider(typeof (Foo), typeof (Bar));

            var settingsReader = new StubSettingsReader(new Dictionary<string, string>
                                                        {
                                                            {"Foo", "FooValue"},
                                                            {"Bar", "BarValue"}
                                                        });
            var settingsOverrider = new StubSettingsOverrider(new Dictionary<string, string>
                                                              {
                                                                  {"Bar", "BarOverriddenValue"}
                                                              });

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

        internal class Foo : ConfigurationSetting<string>
        {
        }

        internal class Bar : ConfigurationSetting<string>
        {
        }

        protected override void When()
        {
            Subject.RegisterConfigurationSettings();
        }

        [Test]
        public void TheNonOverriddenSettingShouldHaveBeenLeftAlone()
        {
            _results
                .OfType<Foo>()
                .Single()
                .Value
                .ShouldBe("FooValue");
        }

        [Test]
        public void TheOverriddenSettingShouldHaveBeenOverridden()
        {
            _results
                .OfType<Bar>()
                .Single()
                .Value
                .ShouldBe("BarOverriddenValue");
        }
    }
}