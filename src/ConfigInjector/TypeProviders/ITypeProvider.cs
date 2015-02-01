using System;
using System.Collections.Generic;

namespace ConfigInjector.TypeProviders
{
    public interface ITypeProvider
    {
        IEnumerable<Type> Get();
    }
}