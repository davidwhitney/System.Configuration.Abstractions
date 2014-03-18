namespace System.Configuration.Abstractions
{
    public interface IConnectionStringInterceptor : IInterceptor
    {
        ConnectionStringSettings OnConnectionStringRetrieve(IAppSettings appSettings, IConnectionStrings connectionStrings, ConnectionStringSettings originalValue);
    }
}