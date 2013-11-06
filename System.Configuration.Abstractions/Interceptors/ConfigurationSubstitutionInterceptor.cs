using System.Collections.Specialized;

namespace System.Configuration.Abstractions.Interceptors
{
    public class ConfigurationSubstitutionInterceptor : IConfigurationInterceptor
    {
        public string OnSettingRetrieve(NameValueCollection appSettings, string originalValue)
        {
            foreach (var key in appSettings.AllKeys)
            {
                var token = "{" + key + "}";

                if (originalValue.Contains(token))
                {
                    originalValue = originalValue.Replace(token, appSettings[key]);
                }
            }

            return originalValue;
        }
    }
}
