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
        #if NET4
        Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap, ConfigurationUserLevel userLevel, bool preLoad);
        #endif
        Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap);
    }
}