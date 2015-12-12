using System;
using System.Collections.Generic;

namespace ConfigInjector.Infrastructure.TypeProviders
{
    public class ExplicitTypeProvider : ITypeProvider
    {
        private readonly Type[] _types;

        public ExplicitTypeProvider(Type[] types)
        {
            _types = types;
        }

        public IEnumerable<Type> Get()
        {
            return _types;
        }
    }
}