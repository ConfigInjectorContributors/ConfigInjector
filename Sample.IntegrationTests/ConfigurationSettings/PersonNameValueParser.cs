using System;
using System.Linq;
using ConfigInjector;
using ConfigInjector.ValueParsers;

namespace Sample.IntegrationTests.ConfigurationSettings
{
    public class PersonNameValueParser : IValueParser
    {
        public int SortOrder
        {
            get { return 100; }
        }

        public bool CanParse(Type settingValueType)
        {
            return typeof (PersonName).IsAssignableFrom(settingValueType);
        }

        public object Parse(Type settingValueType, string settingValueString)
        {
            var tokens = settingValueString.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length < 2) throw new ConfigurationException("A name must have at least two tokens.");

            var firstName = tokens.First();
            var middleNames = string.Join(" ", tokens.Skip(1).Take(tokens.Length - 2));
            var lastName = tokens.Last();

            var result = new PersonName(firstName, middleNames, lastName);
            return result;
        }
    }
}