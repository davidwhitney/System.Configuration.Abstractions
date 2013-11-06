namespace System.Configuration.Abstractions
{
    public interface IAppSettingsExtended : IAppSettings
    {
        string AppSetting(string key);
        T AppSetting<T>(string key);
    }
}