namespace System.Configuration.Abstractions
{
    public interface IConfigurationManager : IConfigurationManagerExtended
    {
        IAppSettings AppSettings { get; set; }
        IConnectionStrings ConnectionStrings { get; set; }
        object GetSection(string sectionName);
        void RefreshSection(string sectionName);
        Configuration OpenExeConfiguration(string exePath);
        Configuration OpenExeConfiguration(ConfigurationUserLevel userLevel);
        Configuration OpenMachineConfiguration();
        Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap, ConfigurationUserLevel userLevel);
        Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap);
        
        #if vLatest
        Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap, ConfigurationUserLevel userLevel, bool preLoad);
        #endif
    }
}