using System;
using System.Collections.Generic;
using System.Linq;
using ThirdDrawer.Extensions;

namespace ConfigInjector.Exceptions
{
    public class ExtraneousSettingsException : ConfigurationException
    {
        public ExtraneousSettingsException(IEnumerable<string> extraneousWebConfigEntries) : base(BuildMessage(extraneousWebConfigEntries))
        {
        }

        private static string BuildMessage(IEnumerable<string> extraneousWebConfigEntries)
        {
            var messages = extraneousWebConfigEntries.Select(k => "Setting {0} appears in [web|app].config but has no corresponding ConfigurationSetting type.".FormatWith(k));
            var message = string.Join(Environment.NewLine, messages);
            return message;
        }
    }
}