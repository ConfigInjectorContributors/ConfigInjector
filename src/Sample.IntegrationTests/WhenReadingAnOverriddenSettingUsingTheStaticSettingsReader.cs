using ConfigInjector.Infrastructure.SettingsOverriders;
using ConfigInjector.QuickAndDirty;
using NUnit.Framework;
using Sample.IntegrationTests.ConfigurationSettings;
using Shouldly;

namespace Sample.IntegrationTests
{
    public class WhenReadingAnOverriddenSettingUsingTheStaticSettingsReader
    {
        private readonly string _environmentVariableKey = EnvironmentVariableSettingsOverrider.DefaultPrefix + "SimpleIntSetting";
        private EnvironmentSettingsMutex _environmentSettingsMutex;

        [SetUp]
        public void SetUp()
        {
            _environmentSettingsMutex = new EnvironmentSettingsMutex();
            _environmentSettingsMutex.SetEnvironmentVariable(_environmentVariableKey, "42");

            DefaultSettingsReader.SetStrategy(new DefaultStaticSettingReaderStrategy());
        }

        [TearDown]
        public void TearDown()
        {
            _environmentSettingsMutex.Dispose();
        }

        [Test]
        public void TheValueShouldBeTheOverriddenOne()
        {
            var setting = DefaultSettingsReader.Get<SimpleIntSetting>();
            setting.Value.ShouldBe(42);
        }
    }
}