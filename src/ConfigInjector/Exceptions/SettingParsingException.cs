using System;
using System.Runtime.Serialization;

namespace ConfigInjector.Exceptions
{
    [Serializable]
    public class SettingParsingException : Exception
    {
        public SettingParsingException()
        {
        }

        public SettingParsingException(string message) : base(message)
        {
        }

        public SettingParsingException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SettingParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}