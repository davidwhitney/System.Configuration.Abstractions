namespace System.Configuration.Abstractions.Interceptors
{
    public class ConfigurationSubstitutionInterceptor : IConfigurationInterceptor, IConnectionStringInterceptor
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

        public ConnectionStringSettings OnConnectionStringRetrieve(IAppSettings appSettings, IConnectionStrings connectionStrings, ConnectionStringSettings originalValue)
        {
            var modifiedConnectionString = OnSettingRetrieve(appSettings, string.Empty, originalValue.ConnectionString);
            originalValue.ConnectionString = modifiedConnectionString;
            return originalValue;
        }
    }
}
