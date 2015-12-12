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
    internal class WhenReadingNestedSettingsFromAnObjectSettingsReader : TestFor<SettingsRegistrationService>
    {
        private object _settingsObject;
        private List<IConfigurationSetting> _results;

        protected override SettingsRegistrationService Given()
        {
            var typeProvider = new StubTypeProvider(typeof (Person_Name), typeof (Person_Drinks), typeof (Milliways_VenueType), typeof (Milliways_Location), typeof (Milliways_Price));

            _settingsObject = new
                              {
                                  Person = new
                                           {
                                               Name = "Arthur Dent",
                                               Drinks = "Tea"
                                           },
                                  Milliways = new
                                              {
                                                  VenueType = "Restaurant",
                                                  Location = "End of the universe",
                                                  Price = 0.01M // One penny invested in your own era with compound interest...
                                              }
                              };

            var settingsReader = new ObjectSettingsReader(_settingsObject);
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

        [Test]
        public void ThePersonShouldBeNamedArthurDent()
        {
            _results
                .OfType<Person_Name>()
                .Single()
                .Value
                .ShouldBe("Arthur Dent");
        }

        [Test]
        public void ArthurDentShouldDrinkTea()
        {
            _results
                .OfType<Person_Drinks>()
                .Single()
                .Value
                .ShouldBe("Tea");
        }

        [Test]
        public void MilliwaysShouldBeARestaurant()
        {
            _results
                .OfType<Milliways_VenueType>()
                .Single()
                .Value
                .ShouldBe("Restaurant");
        }

        [Test]
        public void MilliwaysShouldBeAtTheEndOfTheUniverse()
        {
            _results
                .OfType<Milliways_Location>()
                .Single()
                .Value
                .ShouldBe("End of the universe");
        }

        [Test]
        public void ThePriceShouldBeOnePenny()
        {
            _results
                .OfType<Milliways_Price>()
                .Single()
                .Value
                .ShouldBe(0.01M);
        }

        private class Person_Name : ConfigurationSetting<string>
        {
        }

        private class Person_Drinks : ConfigurationSetting<string>
        {
        }

        private class Milliways_VenueType : ConfigurationSetting<string>
        {
        }

        private class Milliways_Location : ConfigurationSetting<string>
        {
        }

        private class Milliways_Price : ConfigurationSetting<decimal>
        {
        }
    }
}