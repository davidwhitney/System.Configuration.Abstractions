using System.Collections.Specialized;

namespace System.Configuration.Abstractions.Interceptors
{
    public class ConfigurationSubstitutionInterceptor : IConfigurationInterceptor
    {
        public string OnSettingRetrieve(IAppSettings appSettings, string key, string originalValue)
        {
            foreach (var thisKey in appSettings.AllKeys)
            {
                var token = "{" + thisKey + "}";

                if (originalValue.Contains(token))
                {
                    originalValue = originalValue.Replace(token, appSettings[thisKey]);
                }
            }

            return originalValue;
        }
    }
}
