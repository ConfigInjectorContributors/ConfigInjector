using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigInjector.Infrastructure.TypeProviders
{
    public class AssemblyScanningTypeProvider : ITypeProvider
    {
        private readonly Assembly[] _assemblies;

        public AssemblyScanningTypeProvider(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public IEnumerable<Type> Get()
        {
            return _assemblies
                .SelectMany(a => a.GetExportedTypes());
        }
    }
}