using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;

namespace ConfigInjector.UnitTests
{
    [TestFixture]
    public class WhenConvertingConfigurationSettingValues
    {
        [Test]
        [TestCaseSource(typeof (TestCases))]
        public void TheCorrectValuesAndTypesShouldBeReturned(string stringValue, object expectedValue)
        {
            var settingValueType = expectedValue.GetType();
            var settingValue = SettingValueConverter.ParseSettingValue(settingValueType, stringValue);
            settingValue.ShouldBe(expectedValue);
        }

        public class TestCases : IEnumerable<TestCaseData>
        {
            public IEnumerator<TestCaseData> GetEnumerator()
            {
                yield return new TestCaseData("1", 1);
                yield return new TestCaseData("Bar", SomeSettingEnum.Bar);
                yield return new TestCaseData("false", false);
                yield return new TestCaseData("True", true);
                yield return new TestCaseData("1.234", 1.234M);
                yield return new TestCaseData("1.234", 1.234f);
                //yield return new TestCaseData("01:00:00", TimeSpan.FromHours(1));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public enum SomeSettingEnum
        {
            Foo,
            Bar,
            Baz,
        }
    }
}