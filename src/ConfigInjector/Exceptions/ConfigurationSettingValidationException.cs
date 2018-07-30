using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ConfigInjector.Exceptions
{
    [Serializable]
    public class ConfigurationSettingValidationException : ConfigurationException
    {
        public ConfigurationSettingValidationException()
        {
        }

        public ConfigurationSettingValidationException(string[] validationErrors) : base(BuildMessageFromValidationErrors(validationErrors))
        {
            ValidationErrors = validationErrors;
        }

        protected ConfigurationSettingValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IEnumerable<string> ValidationErrors { get; }

        private static string BuildMessageFromValidationErrors(IEnumerable<string> validationErrors)
        {
            var message = string.Join(Environment.NewLine, validationErrors);
            return message;
        }
    }
}