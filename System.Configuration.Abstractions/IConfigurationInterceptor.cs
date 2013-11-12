using System.Collections.Specialized;

namespace System.Configuration.Abstractions
{
    public interface IConfigurationInterceptor
    {
        string OnSettingRetrieve(NameValueCollection appSettings, string key, string originalValue);
    }
}