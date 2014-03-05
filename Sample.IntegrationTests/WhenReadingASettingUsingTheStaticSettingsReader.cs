using ConfigInjector.QuickAndDirty;
using NUnit.Framework;
using Sample.IntegrationTests.ConfigurationSettings;

namespace Sample.IntegrationTests
{
    [TestFixture]
    public class WhenReadingASettingUsingTheStaticSettingsReader
    {
        [Test]
        public void NothingShouldGoBang()
        {
            var setting = DefaultSettingsReader.Get<SimpleIntSetting>();
        }
    }
}