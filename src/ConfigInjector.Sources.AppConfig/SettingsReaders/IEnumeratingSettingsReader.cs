using System.Collections.Generic;

namespace ConfigInjector.Sources.AppConfig.SettingsReaders
{
    public interface IEnumeratingSettingsReader : ISettingsReader
    {
        IEnumerable<string> AllKeys { get; }
    }
}