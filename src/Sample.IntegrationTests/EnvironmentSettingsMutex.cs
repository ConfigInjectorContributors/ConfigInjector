using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Sample.IntegrationTests
{
    public sealed class EnvironmentSettingsMutex : IDisposable
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly Stack<KeyValuePair<string, string>> _toRestore = new Stack<KeyValuePair<string, string>>();

        public EnvironmentSettingsMutex()
        {
            _semaphore.Wait();
        }

        public void SetEnvironmentVariable(string key, string value)
        {
            var oldValue = Environment.GetEnvironmentVariable(key);
            var kvp = new KeyValuePair<string, string>(key, oldValue);
            _toRestore.Push(kvp);

            Environment.SetEnvironmentVariable(key, value);
        }

        public void Dispose()
        {
            while (_toRestore.Any())
            {
                var kvp = _toRestore.Pop();
                Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
            }

            _semaphore.Release();
        }
    }
}