using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace System.Configuration.Abstractions
{
    public class AppSettingsExtended : NameValueCollection, IAppSettings
    {
        private readonly List<IConfigurationInterceptor> _interceptors;

        public AppSettingsExtended(NameValueCollection appSettings, List<IConfigurationInterceptor> interceptors = null)
        {
            _interceptors = interceptors ?? new List<IConfigurationInterceptor>();
            Add(appSettings);
        }

        public string AppSetting(string key, Func<string> whenKeyNotFoundInsteadOfThrowingDefaultException = null)
        {
            return AppSetting<string>(key, whenKeyNotFoundInsteadOfThrowingDefaultException);
        }

        public T AppSetting<T>(string key, Func<T> whenKeyNotFoundInsteadOfThrowingDefaultException = null)
        {
            var rawSetting = this[key];

            if (rawSetting == null)
            {
                if (whenKeyNotFoundInsteadOfThrowingDefaultException != null)
                {
                    return whenKeyNotFoundInsteadOfThrowingDefaultException();
                }

                throw new ConfigurationErrorsException("Calling code requested setting named " + key + " but it was not in the config file.");
            }

            rawSetting = _interceptors.Aggregate(rawSetting,
                (current, interceptor) => interceptor.OnSettingRetrieve(this, key, current));

            return (T) Convert.ChangeType(rawSetting, typeof (T));
        }
    }
}