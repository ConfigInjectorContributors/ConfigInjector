using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Infrastructure;
using ConfigInjector.Infrastructure.Logging;
using ConfigInjector.Infrastructure.SettingsConventions;
using ConfigInjector.Infrastructure.SettingsOverriders;
using ConfigInjector.UnitTests.Stubs;
using NUnit.Framework;

namespace ConfigInjector.UnitTests.Tests.InstrumentationTests
{
    [TestFixture]
    internal class WhenLoggingASetting : TestFor<SettingsRegistrationService>
    {
        private SomeRecordingLogger _logger;

        [SetUp]
        public void TestSetup()
        {
            _logger.LogEntries.Clear();
        }

        protected override SettingsRegistrationService Given()
        {
            var typeProvider = new StubTypeProvider(typeof (SensitiveAndSanitized));

            var settingsReader = new StubSettingsReader(new Dictionary<string, string>
                                                        {
                                                            {typeof(SensitiveAndSanitized).Name, SensitiveAndSanitized.OriginalValueRepresentation},
                                                            {typeof(SensitiveAndUnsanitized).Name, SensitiveAndUnsanitized.OriginalValueRepresentation}
                                                        });
            _logger = new SomeRecordingLogger();

            return new SettingsRegistrationService(_logger,
                                                   typeProvider,
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

        private class SomeRecordingLogger : IConfigInjectorLogger
        {
            public List<string> LogEntries { get; }

            public SomeRecordingLogger()
            {
                LogEntries = new List<string>();
            }

            public void Log(string template, params object[] args)
            {
                LogEntries.Add(string.Format(template, args));
            }
        }

        private class SensitiveAndUnsanitized : ConfigurationSetting<string>
        {
            public const string OriginalValueRepresentation = "SomewhatSensitive";

            public override bool IsSensitive { get { return true; } }
        }

        private class SensitiveAndSanitized : ConfigurationSetting<string>
        {
            public const string OriginalValueRepresentation = "ReallySensitive";
            public const string SanitizedValueRepresentation = "#####";

            public override bool IsSensitive { get { return true; } }
            public override string SanitizedValue { get { return SanitizedValueRepresentation; } }
        }

        [Test]
        public void ASensitiveSettingWithoutASanitizedValueOverrideWillNotMaskTheLogOutput()
        {
            Subject.GetConfigSettingFor(typeof(SensitiveAndUnsanitized));

            CollectionAssert.IsNotEmpty(_logger.LogEntries);

            var logEntry = _logger.LogEntries.First();

            Assert.IsTrue(logEntry.Contains(typeof(SensitiveAndUnsanitized).Name));

            Assert.IsTrue(logEntry.Contains(SensitiveAndUnsanitized.OriginalValueRepresentation));
        }

        [Test]
        public void ASensitiveSettingWithASanitizedValueWillMaskTheLogOutput()
        {
            Subject.GetConfigSettingFor(typeof (SensitiveAndSanitized));

            CollectionAssert.IsNotEmpty(_logger.LogEntries);

            var logEntry = _logger.LogEntries.First();

            Assert.IsTrue(logEntry.Contains(typeof(SensitiveAndSanitized).Name));

            Assert.IsFalse(logEntry.Contains(SensitiveAndSanitized.OriginalValueRepresentation));
            Assert.IsTrue(logEntry.Contains(SensitiveAndSanitized.SanitizedValueRepresentation));
        }
    }
}