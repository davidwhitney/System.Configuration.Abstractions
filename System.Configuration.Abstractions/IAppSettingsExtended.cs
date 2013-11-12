namespace System.Configuration.Abstractions
{
    public interface IAppSettingsExtended
    {
        string AppSetting(string key, Func<string> whenKeyNotFoundInsteadOfThrowingDefaultException = null);
        T AppSetting<T>(string key, Func<T> whenKeyNotFoundInsteadOfThrowingDefaultException = null);
    }
}