using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Configuration.Abstractions
{
    public class ConnectionStringsExtended : IConnectionStrings
    {
        public ConnectionStringSettingsCollection Raw { get; }

        private readonly IEnumerable<IConnectionStringInterceptor> _interceptors;
        private readonly IAppSettings _appSettings;

        public ConnectionStringsExtended(ConnectionStringSettingsCollection raw, IAppSettings appSettings, IEnumerable<IConnectionStringInterceptor> interceptors = null)
        {
            _appSettings = appSettings;
            _interceptors = interceptors ?? new List<IConnectionStringInterceptor>();
            Raw = raw;
        }

        public ConnectionStringSettings this[string name] => Intercept(Raw[name]);

        public ConnectionStringSettings this[int index] => Intercept(Raw[index]);

        public int IndexOf(ConnectionStringSettings settings) => Raw.IndexOf(settings);
        public void Add(ConnectionStringSettings settings) => Raw.Add(settings);
        public void Remove(ConnectionStringSettings settings) => Raw.Remove(settings);
        public void RemoveAt(int index) => Raw.RemoveAt(index);
        public void Remove(string name) => Raw.Remove(name);
        public void Clear() => Raw.Clear();

        private ConnectionStringSettings Intercept(ConnectionStringSettings rawSetting) 
            => _interceptors.Aggregate(rawSetting, (current, interceptor) => interceptor.OnConnectionStringRetrieve(_appSettings, this, current));

        IEnumerator IEnumerable.GetEnumerator() => Raw.GetEnumerator();

        public IEnumerator<ConnectionStringSettings> GetEnumerator()
        {
            for (var pos = 0; pos < Raw.Count; pos++)
            {
                yield return Intercept(Raw[pos]);
            }
        }
    }
}