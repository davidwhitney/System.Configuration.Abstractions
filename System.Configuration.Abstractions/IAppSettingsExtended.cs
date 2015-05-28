using System.Collections.Specialized;

namespace System.Configuration.Abstractions
{
    public interface IAppSettingsExtended
    {
        NameValueCollection Raw { get; }
        T AppSettingSilent<T>(string key, Func<T> insteadOfThrowingDefaultException = null);
        T AppSettingConvert<T>(string key, Func<T> whenConversionFailsInsteadOfThrowingDefaultException = null);
        string AppSetting(string key, Func<string> whenKeyNotFoundInsteadOfThrowingDefaultException = null);
        T AppSetting<T>(string key, Func<T> whenKeyNotFoundInsteadOfThrowingDefaultException = null, Func<T> whenConversionFailsInsteadOfThrowingDefaultException = null);
        TSettingsDto Map<TSettingsDto>() where TSettingsDto : class, new();
    }
}