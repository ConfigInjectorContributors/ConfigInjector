using System;
using ConfigInjector.Sources.AppConfig.Exceptions;
using NUnit.Framework;
using Shouldly;

namespace ConfigInjector.Sources.AppConfig.Tests.Sources.AppConfig.ValueConversionTests
{
    [TestFixture]
    public class WhenParsingASettingThatHasAnInvalidValue
    {
        [Test]
        [TestCase(typeof(int), "foo")]
        [TestCase(typeof(Guid), "foo")]
        public void WeShouldThrowAnException(Type settingValueType, string settingValue)
        {
            var converter = new SettingValueConverter();

            Should.Throw<SettingParsingException>(() => converter.ParseSettingValue(settingValueType, settingValue));
        }
    }
}