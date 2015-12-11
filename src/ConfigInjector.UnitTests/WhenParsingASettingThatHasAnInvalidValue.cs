using System;
using ConfigInjector.Exceptions;
using ConfigInjector.Infrastructure;
using NUnit.Framework;

namespace ConfigInjector.UnitTests
{
    [TestFixture]
    public class WhenParsingASettingThatHasAnInvalidValue
    {
        [Test]
        [TestCase(typeof (int), "foo")]
        [TestCase(typeof (Guid), "foo")]
        [ExpectedException(typeof (SettingParsingException))]
        public void WeShouldThrowAnException(Type settingValueType, string settingValue)
        {
            var converter = new SettingValueConverter();
            converter.ParseSettingValue(settingValueType, settingValue);
        }
    }
}