using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ConfigInjector
{
    [Serializable]
    public class ConfigurationSettingValidationException : Exception
    {
        private readonly IEnumerable<string> _validationErrors;

        public ConfigurationSettingValidationException()
        {
        }

        public ConfigurationSettingValidationException(string[] validationErrors) : base(BuildMessageFromValidationErrors(validationErrors))
        {
            _validationErrors = validationErrors;
        }

        protected ConfigurationSettingValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IEnumerable<string> ValidationErrors
        {
            get { return _validationErrors; }
        }

        private static string BuildMessageFromValidationErrors(IEnumerable<string> validationErrors)
        {
            var message = string.Join(Environment.NewLine, validationErrors);
            return message;
        }
    }
}