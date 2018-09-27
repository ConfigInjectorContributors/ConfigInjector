using System;
using System.Collections.Generic;
using System.Text;
using ConfigInjector.Exceptions;

namespace ConfigInjector.Sources.AppConfig.Exceptions
{
    [Serializable]
    public class AmbiguousSettingException : ConfigurationException
    {
        public AmbiguousSettingException(Type settingType, Dictionary<string, string> candidateSettings)
            : base(string.Format("Setting {0} is ambiguous and has more than one match (Candidates are {1})", settingType.FullName, JoinCandidates(candidateSettings)))
        {
        }

        private static string JoinCandidates(Dictionary<string, string> matchingSettings)
        {
            var sb = new StringBuilder();
            foreach (var kvp in matchingSettings)
            {
                sb.AppendFormat("{0}: {1}", kvp.Key, kvp.Value);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}