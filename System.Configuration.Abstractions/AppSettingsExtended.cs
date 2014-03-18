using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;

namespace System.Configuration.Abstractions
{
    public class AppSettingsExtended : NameObjectCollectionBase, IAppSettings
    {
        public NameValueCollection Raw { get; private set; }
        private readonly IEnumerable<IConfigurationInterceptor> _interceptors;

        public AppSettingsExtended(NameValueCollection raw, IEnumerable<IConfigurationInterceptor> interceptors = null)
        {
            Raw = raw;
            _interceptors = interceptors ?? new List<IConfigurationInterceptor>();
        }

        public string AppSetting(string key, Func<string> whenKeyNotFoundInsteadOfThrowingDefaultException = null)
        {
            return AppSetting<string>(key, whenKeyNotFoundInsteadOfThrowingDefaultException);
        }

        public T AppSetting<T>(string key, Func<T> whenKeyNotFoundInsteadOfThrowingDefaultException = null)
        {
            var rawSetting = Raw[key];

            if (rawSetting == null)
            {
                if (whenKeyNotFoundInsteadOfThrowingDefaultException != null)
                {
                    return whenKeyNotFoundInsteadOfThrowingDefaultException();
                }

                throw new ConfigurationErrorsException("Calling code requested setting named " + key +
                                                       " but it was not in the config file.");
            }

            rawSetting = Intercept(key, rawSetting);

            return (T) Convert.ChangeType(rawSetting, typeof (T));
        }

        private string Intercept(string key, string rawSetting)
        {
            rawSetting = _interceptors.Aggregate(rawSetting,
                (current, interceptor) => interceptor.OnSettingRetrieve(this, key, current));
            return rawSetting;
        }

        public void Add(NameValueCollection c)
        {
            Raw.Add(c);
        }

        public void Clear()
        {
            Raw.Clear();
        }

        public void CopyTo(Array dest, int index)
        {
            Raw.CopyTo(dest, index);
        }

        public bool HasKeys()
        {
            return Raw.HasKeys();
        }

        public void Add(string name, string value)
        {
            Raw.Add(name, value);
        }

        public string Get(string name)
        {
            return Intercept(name, Raw.Get(name));
        }

        public string[] GetValues(string name)
        {
            var values = Raw.GetValues(name);
            return values == null
                ? null
                : values.Select(value => Intercept(name, value)).ToArray();
        }

        public void Set(string name, string value)
        {
            Raw.Set(name, value);
        }

        public void Remove(string name)
        {
            Raw.Remove(name);
        }

        public string Get(int index)
        {
            return Intercept(index.ToString(), Raw.Get(index));
        }

        public string[] GetValues(int index)
        {
            var values = Raw.GetValues(index);
            return values == null
                ? null
                : values.Select(value => Intercept(index.ToString(), value)).ToArray();
        }

        public string GetKey(int index)
        {
            return Raw.GetKey(index);
        }

        public string this[string key]
        {
            get { return Intercept(key, Raw[key]); }
            set { Raw[key] = value; }
        }

        public string this[int index]
        {
            get { return Intercept(index.ToString(), Raw[index]); }
        }

        public string[] AllKeys
        {
            get { return Raw.AllKeys; }
        }

        public override int Count
        {
            get { return Raw.Count; }
        }

        public override KeysCollection Keys
        {
            get { return Raw.Keys; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Raw.GetObjectData(info, context);
        }

        public override void OnDeserialization(object sender)
        {
            Raw.OnDeserialization(sender);
        }

        IEnumerator IAppSettings.GetEnumerator()
        {
            return Raw.GetEnumerator();
        }
    }
}