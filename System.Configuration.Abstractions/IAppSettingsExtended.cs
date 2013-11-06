namespace System.Configuration.Abstractions
{
    public interface IAppSettingsExtended
    {
        string AppSetting(string key);
        T AppSetting<T>(string key);
    }
}