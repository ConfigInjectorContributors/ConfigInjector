using System;
using System.Collections.Generic;
using System.Linq;
using ConfigInjector.Extensions;

namespace ConfigInjector.Infrastructure.SettingsConventions
{
    public static class SettingKeyConventions
    {
        public static IEnumerable<ISettingKeyConvention> BuiltInConventions
        {
            get
            {
                var builtInConventions = typeof(SettingKeyConventions).Assembly
                                                                      .DefinedTypes
                                                                      .Where(t => t.IsAssignableTo<ISettingKeyConvention>())
                                                                      .Where(t => t.IsInstantiable())
                                                                      .Select(Activator.CreateInstance)
                                                                      .Cast<ISettingKeyConvention>()
                                                                      .ToArray();

                return builtInConventions;
            }
        }
    }
}