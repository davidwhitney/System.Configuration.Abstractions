using System.Collections.Specialized;

namespace System.Configuration.Abstractions
{
    public interface IConfigurationInterceptor
    {
        string OnSettingRetrieve(IAppSettings appSettings, string key, string originalValue);
    }
}