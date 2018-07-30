using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigInjector.Infrastructure.TypeProviders
{
    public class FilteredAssemblyScanningTypeProvider : ITypeProvider
    {
        private readonly Func<Type, bool> _filter;
        private readonly Assembly[] _assemblies;

        public FilteredAssemblyScanningTypeProvider(Func<Type, bool> filter, params Assembly[] assemblies)
        {
            _filter = filter;
            _assemblies = assemblies;
        }

        public IEnumerable<Type> Get()
        {
            return _assemblies
                   .SelectMany(a => a.GetExportedTypes())
                   .Where(_filter);
        }
    }
}