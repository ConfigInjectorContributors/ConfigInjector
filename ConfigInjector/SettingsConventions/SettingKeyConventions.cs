using System.Collections.Generic;

namespace ConfigInjector.SettingsConventions
{
    public static class SettingKeyConventions
    {
        public static IEnumerable<ISettingKeyConvention> BuiltInConventions
        {
            get
            {
                yield return new DefaultSettingKeyConvention();
                yield return new WithSuffixSettingKeyConvention();
            }
        }
    }
}