using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigInjector.Exceptions
{
    public class ExtraneousSettingsException : ConfigurationException
    {
        public ExtraneousSettingsException(IEnumerable<string> extraneousWebConfigEntries) : base(BuildMessage(extraneousWebConfigEntries))
        {
        }

        private static string BuildMessage(IEnumerable<string> extraneousWebConfigEntries)
        {
            var messages = extraneousWebConfigEntries.Select(k => string.Format("Setting {0} appears in [web|app].config but has no corresponding ConfigurationSetting type.", k));
            var message = string.Join(Environment.NewLine, messages);
            return message;
        }
    }
}