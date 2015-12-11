using System.Collections.Generic;

namespace ConfigInjector.Infrastructure.SettingsReaders
{
    public interface ISettingsReader
    {
        string ReadValue(string key);
        IEnumerable<string> AllKeys { get; }
    }
}