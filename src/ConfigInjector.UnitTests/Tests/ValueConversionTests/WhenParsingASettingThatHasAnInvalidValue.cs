using System;
using ConfigInjector.Exceptions;
using ConfigInjector.Infrastructure;
using NUnit.Framework;
using Shouldly;

namespace ConfigInjector.UnitTests.Tests.ValueConversionTests
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