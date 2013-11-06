using System.Collections.Specialized;

namespace System.Configuration.Abstractions
{
    public class AppSettingsExtended : NameValueCollection, IAppSettingsExtended
    {
        public AppSettingsExtended() : this(System.Configuration.ConfigurationManager.AppSettings)
        {
        }

        public AppSettingsExtended(NameValueCollection appSettings)
        {
            Add(appSettings);
        }

        public string AppSetting(string key)
        {
            return AppSetting<string>(key);
        }

        public T AppSetting<T>(string key)
        {
            var rawSetting = this[key];

            if (rawSetting == null)
            {
                throw new ConfigurationErrorsException("Calling code requested setting named " + key + " but it was not in the config file.");
            }

            return (T) Convert.ChangeType(rawSetting, typeof (T));
        }
    }
}