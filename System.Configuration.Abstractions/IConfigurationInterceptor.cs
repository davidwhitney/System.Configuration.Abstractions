namespace System.Configuration.Abstractions
{
    public interface IConfigurationInterceptor : IInterceptor
    {
        string OnSettingRetrieve(IAppSettings appSettings, string key, string originalValue);
    }
}