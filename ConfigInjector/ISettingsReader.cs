using System.Collections.Generic;

namespace ConfigInjector
{
    public interface ISettingsReader
    {
        string ReadValue(string key);
        IEnumerable<string> AllKeys { get; }
    }
}