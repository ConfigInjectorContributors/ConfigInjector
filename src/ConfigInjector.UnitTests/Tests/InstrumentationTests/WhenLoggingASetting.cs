using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Infrastructure;
using ConfigInjector.Infrastructure.SettingsConventions;
using ConfigInjector.Infrastructure.SettingsOverriders;
using ConfigInjector.UnitTests.Stubs;
using NUnit.Framework;
using Shouldly;

namespace ConfigInjector.UnitTests.Tests.InstrumentationTests
{
    [TestFixture]
    internal class WhenLoggingASetting : TestFor<SettingsRegistrationService>
    {
        private MemoryLogger _logger;
        private StubTypeProvider _typeProvider;

        protected override SettingsRegistrationService Given()
        {
            _logger = new MemoryLogger();
            _typeProvider = new StubTypeProvider(typeof(SensitiveAndSanitized));

            var settingsReader = new StubSettingsReader(new Dictionary<string, string>
                                                        {
                                                            {typeof(SensitiveAndSanitized).Name, SensitiveAndSanitized.OriginalValueRepresentation},
                                                            {typeof(SensitiveAndUnsanitized).Name, SensitiveAndUnsanitized.OriginalValueRepresentation}
                                                        });

            return new SettingsRegistrationService(_logger,
                                                   _typeProvider,
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

        private class SensitiveAndUnsanitized : ConfigurationSetting<string>
        {
            public const string OriginalValueRepresentation = "SomewhatSensitive";

            public override bool IsSensitive => true;
        }

        private class SensitiveAndSanitized : ConfigurationSetting<string>
        {
            public const string OriginalValueRepresentation = "ReallySensitive";
            public const string SanitizedValueRepresentation = "#####";

            public override bool IsSensitive => true;
            public override string SanitizedValue => SanitizedValueRepresentation;
        }

        [Test]
        public void ASensitiveSettingWithoutASanitizedValueOverrideWillNotMaskTheLogOutput()
        {
            Subject.GetConfigSettingFor(typeof(SensitiveAndUnsanitized));

            var logEntry = _logger.LogEntries.Single();

            logEntry.Args.ShouldContain(typeof(SensitiveAndUnsanitized).Name);
            logEntry.Args.ShouldContain(SensitiveAndUnsanitized.OriginalValueRepresentation);
        }

        [Test]
        public void ASensitiveSettingWithASanitizedValueWillMaskTheLogOutput()
        {
            Subject.GetConfigSettingFor(typeof(SensitiveAndSanitized));

            var logEntry = _logger.LogEntries.Single();

            logEntry.Args.ShouldContain(typeof(SensitiveAndSanitized).Name);
            logEntry.Args.ShouldNotContain(SensitiveAndSanitized.OriginalValueRepresentation);
            logEntry.Args.ShouldContain(SensitiveAndSanitized.SanitizedValueRepresentation);
        }
    }
}