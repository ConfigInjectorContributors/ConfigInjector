namespace Sample.IntegrationTests.ConfigurationSettings
{
    public class PersonName
    {
        private readonly string _firstName;
        private readonly string _middleNames;
        private readonly string _lastName;

        public PersonName(string firstName, string middleNames, string lastName)
        {
            _firstName = firstName;
            _middleNames = middleNames;
            _lastName = lastName;
        }

        public string FirstName
        {
            get { return _firstName; }
        }

        public string MiddleNames
        {
            get { return _middleNames; }
        }

        public string LastName
        {
            get { return _lastName; }
        }
    }
}