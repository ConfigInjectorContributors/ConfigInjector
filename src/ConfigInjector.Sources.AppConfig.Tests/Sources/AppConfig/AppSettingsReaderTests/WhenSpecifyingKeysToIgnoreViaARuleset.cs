using System;
using System.Collections.Generic;
using ConfigInjector.Tests.Common;
using NUnit.Framework;
using Shouldly;

namespace ConfigInjector.Sources.AppConfig.Tests.Sources.AppConfig.AppSettingsReaderTests
{
    public class WhenSpecifyingKeysToIgnoreViaARuleset : TestFor<AppSettingsReader>
    {
        private IEnumerable<string> _keys;

        protected override AppSettingsReader Given()
        {
            var rule = (Func<string, bool>) (k => k.StartsWith("ba"));
            return new AppSettingsReader(new[] {rule});
        }

        protected override void When()
        {
            _keys = Subject.AllKeys;
        }

        [Test]
        public void ItShouldIgnoreKeysThatMatchTheRule()
        {
            _keys.ShouldBe(new[] {"foo"});
        }
    }
}