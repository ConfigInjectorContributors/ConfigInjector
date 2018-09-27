using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;

namespace ConfigInjector.Sources.AppConfig.Tests.Sources.AppConfig.ValueConversionTests
{
    [TestFixture]
    public class WhenConvertingConfigurationSettingValues
    {
        [Test]
        [TestCaseSource(typeof (TestCases))]
        public void TheCorrectValuesAndTypesShouldBeReturned(string stringValue, object expectedValue, Type settingValueType)
        {
            var settingValue = new SettingValueConverter().ParseSettingValue(settingValueType, stringValue);
            settingValue.ShouldBe(expectedValue);
        }

        public class TestCases : IEnumerable<TestCaseData>
        {
            public IEnumerator<TestCaseData> GetEnumerator()
            {
                yield return TestCase("1", 1);
                yield return TestCase("Bar", SomeSettingEnum.Bar);
                yield return TestCase("false", false);
                yield return TestCase("True", true);
                yield return TestCase("1.234", 1.234M);
                yield return TestCase("1.234", 1.234f);
                yield return TestCase("01:00:00", TimeSpan.FromHours(1));
                yield return TestCase("1970-01-01", new DateTime(1970, 01, 01));  // Unix epoch FTW!
                yield return TestCase("http://www.codingforfunandprofit.com/", new Uri("http://www.codingforfunandprofit.com/"));
                yield return TestCase("7", new SomeCustomValueTypeThatLooksSuspiciouslyLikeAnInteger(7));
                yield return TestCase("", null, typeof(int?));
                yield return TestCase("1", 1, typeof(int?));
                yield return TestCase("", null, typeof(Guid?));
                yield return TestCase("00000000-0000-0000-0000-000000000000", Guid.Empty, typeof(Guid?));
            }

            public TestCaseData TestCase(string stringValue, object expectedValue)
            {
                return TestCase(stringValue, expectedValue, expectedValue.GetType());
            }

            public TestCaseData TestCase(string stringValue, object expectedValue, Type settingValueType)
            {
                return new TestCaseData(stringValue, expectedValue, settingValueType);
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

        public class SomeCustomValueTypeThatLooksSuspiciouslyLikeAnInteger
        {
            private readonly int _value;

            public SomeCustomValueTypeThatLooksSuspiciouslyLikeAnInteger(int value)
            {
                _value = value;
            }

            public int Value
            {
                get { return _value; }
            }

            public static SomeCustomValueTypeThatLooksSuspiciouslyLikeAnInteger Parse(string stringValue)
            {
                return new SomeCustomValueTypeThatLooksSuspiciouslyLikeAnInteger(int.Parse(stringValue));
            }

            public override bool Equals(object obj)
            {
                var myType = obj as SomeCustomValueTypeThatLooksSuspiciouslyLikeAnInteger;
                if (myType == null) return false;
                return _value == myType.Value;
            }

            public override int GetHashCode()
            {
                return _value.GetHashCode();
            }
        }
    }
}