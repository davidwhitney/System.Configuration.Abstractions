namespace System.Configuration.Abstractions
{
    public interface IConfigurationManagerExtended
    {
        T GetSection<T>(string sectionName);
    }
}