using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;

namespace System.Configuration.Abstractions
{
    public class AppSettingsExtended : IAppSettings
    {
        private readonly NameValueCollection _appSettings;
        private readonly List<IConfigurationInterceptor> _interceptors;

        public AppSettingsExtended(NameValueCollection appSettings, List<IConfigurationInterceptor> interceptors = null)
        {
            _appSettings = appSettings;
            _interceptors = interceptors ?? new List<IConfigurationInterceptor>();
        }

        public string AppSetting(string key, Func<string> whenKeyNotFoundInsteadOfThrowingDefaultException = null)
        {
            return AppSetting<string>(key, whenKeyNotFoundInsteadOfThrowingDefaultException);
        }

        public T AppSetting<T>(string key, Func<T> whenKeyNotFoundInsteadOfThrowingDefaultException = null)
        {
            var rawSetting = _appSettings[key];

            if (rawSetting == null)
            {
                if (whenKeyNotFoundInsteadOfThrowingDefaultException != null)
                {
                    return whenKeyNotFoundInsteadOfThrowingDefaultException();
                }

                throw new ConfigurationErrorsException("Calling code requested setting named " + key + " but it was not in the config file.");
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
            _appSettings.Add(c);
        }

        public void Clear()
        {
            _appSettings.Clear();
        }

        public void CopyTo(Array dest, int index)
        {
            _appSettings.CopyTo(dest, index);
        }

        public bool HasKeys()
        {
            return _appSettings.HasKeys();
        }

        public void Add(string name, string value)
        {
            _appSettings.Add(name, value);
        }

        public string Get(string name)
        {
            return Intercept(name, _appSettings.Get(name));
        }

        public string[] GetValues(string name)
        {
            return _appSettings.GetValues(name);
        }

        public void Set(string name, string value)
        {
            _appSettings.Set(name, value);
        }

        public void Remove(string name)
        {
            _appSettings.Remove(name);
        }

        public string Get(int index)
        {
            return Intercept(index.ToString(), _appSettings.Get(index));
        }

        public string[] GetValues(int index)
        {
            return _appSettings.GetValues(index);
        }

        public string GetKey(int index)
        {
            return _appSettings.GetKey(index);
        }

        public string this[string key]
        {
            get { return Intercept(key, _appSettings[key]); }
            set { _appSettings[key] = value; }
        }

        public string this[int index]
        {
            get { return Intercept(index.ToString(), _appSettings[index]); }
            set { throw new NotImplementedException(""); }
        }

        string IAppSettings.this[string name]
        {
            get { return Intercept(name, _appSettings[name]); }
            set { _appSettings[name] = value; }
        }

        string IAppSettings.this[int index]
        {
            get { return Intercept(index.ToString(), _appSettings[index]); }
        }

        public string[] AllKeys
        {
            get { return _appSettings.AllKeys; }
        }

        public int Count
        {
            get { return _appSettings.Count; }
        }

        public NameObjectCollectionBase.KeysCollection Keys
        {
            get { return _appSettings.Keys; }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _appSettings.GetObjectData(info, context);
        }

        public void OnDeserialization(object sender)
        {
            _appSettings.OnDeserialization(sender);
        }

        public IEnumerator GetEnumerator()
        {
            return _appSettings.GetEnumerator();
        }
    }
}