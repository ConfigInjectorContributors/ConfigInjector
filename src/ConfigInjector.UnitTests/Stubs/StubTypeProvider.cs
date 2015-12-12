using System;
using System.Collections.Generic;
using ConfigInjector.Infrastructure.TypeProviders;

namespace ConfigInjector.UnitTests.Stubs
{
    internal class StubTypeProvider : ITypeProvider
    {
        private readonly Type[] _settingTypes;

        public StubTypeProvider(params Type[] settingTypes)
        {
            _settingTypes = settingTypes;
        }

        public IEnumerable<Type> Get()
        {
            return _settingTypes;
        }
    }
}