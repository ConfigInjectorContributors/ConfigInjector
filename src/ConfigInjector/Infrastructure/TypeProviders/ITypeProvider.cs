using System;
using System.Collections.Generic;

namespace ConfigInjector.Infrastructure.TypeProviders
{
    public interface ITypeProvider
    {
        IEnumerable<Type> Get();
    }
}