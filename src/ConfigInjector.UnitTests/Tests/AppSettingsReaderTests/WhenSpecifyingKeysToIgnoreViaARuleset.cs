using System.Collections.Generic;
using ConfigInjector.Infrastructure.SettingsReaders;
using NUnit.Framework;
using Shouldly;

namespace ConfigInjector.UnitTests.Tests.AppSettingsReaderTests
{
    public class WhenSpecifyingKeysToIgnoreViaARuleset : TestFor<AppSettingsReader>
    {
        private IEnumerable<string> _keys;

        protected override AppSettingsReader Given()
        {
            return new AppSettingsReader(k => k.StartsWith("ba"));
        }

        protected override void When()
        {
            _keys = Subject.AllKeys;
        }

        [Test]
        public void ItShouldIgnoreKeysThatMatchTheRule()
        {
            _keys.ShouldBe(new []{"foo"});
        }
    }
}