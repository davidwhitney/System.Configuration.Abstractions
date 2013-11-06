namespace System.Configuration.Abstractions
{
    public interface IConfigurationManagerExtended : IConfigurationManager
    {
        T GetSection<T>(string sectionName);
    }
}