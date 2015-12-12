using System.Collections.Generic;

namespace ConfigInjector.Infrastructure.SettingsReaders
{
    public interface IEnumeratingSettingsReader : ISettingsReader
    {
        IEnumerable<string> AllKeys { get; }
    }
}