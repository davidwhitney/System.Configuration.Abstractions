namespace System.Configuration.Abstractions
{
    public interface IConfigurationInterceptor
    {
        string OnSettingRetrieve(string originalValue);
    }
}