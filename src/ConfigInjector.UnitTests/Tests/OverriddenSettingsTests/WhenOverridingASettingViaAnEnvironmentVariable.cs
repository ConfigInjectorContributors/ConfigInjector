using System;
using ConfigInjector.Infrastructure.SettingsOverriders;
using NUnit.Framework;
using Shouldly;

namespace ConfigInjector.UnitTests.Tests.OverriddenSettingsTests
{
    public class WhenOverridingASettingViaAnEnvironmentVariable : TestFor<EnvironmentVariableSettingsOverrider>
    {
        private const string _prefix = "AppSetting_";
        private const string _variable = "SomeSetting";

        private string _actualValue;
        private string _expectedValue;
        private bool _isOverridden;

        protected override EnvironmentVariableSettingsOverrider Given()
        {
            return new EnvironmentVariableSettingsOverrider(_prefix);
        }

        protected override void When()
        {
            _expectedValue = "SomeValue";
            Environment.SetEnvironmentVariable(_prefix + _variable, _expectedValue);

            _isOverridden = Subject.TryFindOverrideFor(_variable, out _actualValue);
        }

        [Test]
        public void TheValueShouldBeOverridden()
        {
            _isOverridden.ShouldBe(true);
        }

        [Test]
        public void TheRetrievedValueShouldBeCorrect()
        {
            _actualValue.ShouldBe(_expectedValue);
        }
    }
}