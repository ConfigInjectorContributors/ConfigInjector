using System;
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
    internal class WhenLoggingAnOverridenSetting : TestFor<SettingsRegistrationService>
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
                                                   new EnvironmentVariableSettingsOverrider(),
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
            public const string OverridenValueRepresentation = "OverridenAndSomewhatSensitive";

            public override bool IsSensitive => true;
        }

        private class SensitiveAndSanitized : ConfigurationSetting<string>
        {
            public const string OriginalValueRepresentation = "ReallySensitive";
            public const string OverridenValueRepresentation = "OverridenAndReallySensitive";
            public const string SanitizedValueRepresentation = "#####";

            public override bool IsSensitive => true;
            public override string SanitizedValue => SanitizedValueRepresentation;
        }

        [Test]
        public void AnOverridenSensitiveSettingWithoutASanitizedValueOverrideWillNotMaskTheLogOutput()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableSettingsOverrider.DefaultPrefix + "SensitiveAndUnsanitized", SensitiveAndUnsanitized.OverridenValueRepresentation);
            Subject.GetConfigSettingFor(typeof(SensitiveAndUnsanitized));

            _logger.LogEntries.ShouldNotBeEmpty();

            var overriddenLogEntry = _logger.LogEntries.Last();
            overriddenLogEntry.Args.ShouldContain(typeof(SensitiveAndUnsanitized).Name);
            overriddenLogEntry.Args.ShouldNotContain(SensitiveAndUnsanitized.OriginalValueRepresentation);
            overriddenLogEntry.Args.ShouldContain(SensitiveAndUnsanitized.OverridenValueRepresentation);
        }

        [Test]
        public void AnOverridenSensitiveSettingWithASanitizedValueWillMaskTheLogOutput()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariableSettingsOverrider.DefaultPrefix + "SensitiveAndSanitized", SensitiveAndSanitized.OverridenValueRepresentation);

            Subject.GetConfigSettingFor(typeof(SensitiveAndSanitized));

            _logger.LogEntries.ShouldNotBeEmpty();
            foreach (var logEntry in _logger.LogEntries)
            {
                logEntry.Args.ShouldContain(typeof(SensitiveAndSanitized).Name);
                logEntry.Args.ShouldNotContain(SensitiveAndSanitized.OriginalValueRepresentation);
                logEntry.Args.ShouldNotContain(SensitiveAndSanitized.OverridenValueRepresentation);
                logEntry.Args.ShouldContain(SensitiveAndSanitized.SanitizedValueRepresentation);
            }
        }
    }
}