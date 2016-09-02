using System.Collections.Generic;
using ConfigInjector.Infrastructure.SettingsReaders;
using NUnit.Framework;
using Shouldly;

namespace ConfigInjector.UnitTests.Tests.AppSettingsReaderTests
{
    public class WhenSpecifyingKeysToIgnore : TestFor<AppSettingsReader>
    {
        private IEnumerable<string> _keys;

        protected override AppSettingsReader Given()
        {
            return new AppSettingsReader(new[]{"bar"});
        }

        protected override void When()
        {
            _keys = Subject.AllKeys;
        }

        [Test]
        public void ItShouldIgnoreNamedKeys()
        {
            _keys.ShouldBe(new [] {"foo", "baz"});
        }
    }
}