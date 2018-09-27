using System;
using Autofac;

namespace ConfigInjector.Containers.Autofac
{
    public class AutofacDependencyResolver : IDependencyResolver
    {
        private readonly IComponentContext _componentContext;

        public AutofacDependencyResolver(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public bool TryResolve(Type type, out object instance)
        {
            return _componentContext.TryResolve(type, out instance);
        }
    }
}