using System;

namespace ConfigInjector
{
    public interface IDependencyResolver
    {
        bool TryResolve(Type type, out object instance);
    }
}