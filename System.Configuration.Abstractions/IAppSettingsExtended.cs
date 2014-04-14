using System.Collections.Specialized;

namespace System.Configuration.Abstractions
{
    public interface IAppSettingsExtended
    {
        NameValueCollection Raw { get; }

        string AppSetting(string key, Func<string> whenKeyNotFoundInsteadOfThrowingDefaultException = null);
        T AppSetting<T>(string key, Func<T> whenKeyNotFoundInsteadOfThrowingDefaultException = null);
        TSettingsDto Map<TSettingsDto>() where TSettingsDto : class, new();
    }
}