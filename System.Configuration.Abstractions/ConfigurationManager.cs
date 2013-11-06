using System.Collections.Generic;
using System.Collections.Specialized;

namespace System.Configuration.Abstractions
{
    public class ConfigurationManager : IConfigurationManager
    {
        public IAppSettings AppSettings { get; set; }
        public ConnectionStringSettingsCollection ConnectionStrings { get; set; }
        public static List<IConfigurationInterceptor> Interceptors { get; private set; }
        
        static ConfigurationManager()
        {
            Interceptors = new List<IConfigurationInterceptor>();
        }

        public static void RegisterInterceptors(params IConfigurationInterceptor[] interceptors)
        {
            Interceptors.AddRange(interceptors);
        }

        public ConfigurationManager() :
            this(System.Configuration.ConfigurationManager.AppSettings,
                System.Configuration.ConfigurationManager.ConnectionStrings)
        {
        }

        public ConfigurationManager(NameValueCollection appSettingss)
            : this(appSettingss, System.Configuration.ConfigurationManager.ConnectionStrings)
        {
        }

        public ConfigurationManager(NameValueCollection appSettings, ConnectionStringSettingsCollection connectionStringSettings)
        {
            AppSettings = new AppSettingsExtended(appSettings, Interceptors);
            ConnectionStrings = connectionStringSettings;
        }

        public object GetSection(string sectionName)
        {
            return System.Configuration.ConfigurationManager.GetSection(sectionName);
        }

        public T GetSection<T>(string sectionName)
        {
            return (T)System.Configuration.ConfigurationManager.GetSection(sectionName);
        }

        public void RefreshSection(string sectionName)
        {
            System.Configuration.ConfigurationManager.RefreshSection(sectionName);
        }

        public Configuration OpenExeConfiguration(string exePath)
        {
            return System.Configuration.ConfigurationManager.OpenExeConfiguration(exePath);
        }

        public Configuration OpenExeConfiguration(ConfigurationUserLevel userLevel)
        {
            return System.Configuration.ConfigurationManager.OpenExeConfiguration(userLevel);
        }

        public Configuration OpenMachineConfiguration()
        {
            return System.Configuration.ConfigurationManager.OpenMachineConfiguration();
        }

        public Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap, ConfigurationUserLevel userLevel)
        {
            return System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, userLevel);
        }

        public Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap, ConfigurationUserLevel userLevel, bool preLoad)
        {
            return System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, userLevel, preLoad);
        }

        public Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap)
        {
            return System.Configuration.ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
        }

        [Obsolete("Exists for in-place switching of System.Configuration.ConfigurationManager - avoid this static helper")]
        public static IConfigurationManager Instance
        {
            get { return new ConfigurationManager(); }
        }
    }
}
