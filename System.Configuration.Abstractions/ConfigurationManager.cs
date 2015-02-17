using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace System.Configuration.Abstractions
{
    public class ConfigurationManager : IConfigurationManager
    {
        public IAppSettings AppSettings { get; set; }
        public IConnectionStrings ConnectionStrings { get; set; }

        public static List<IInterceptor> Interceptors { get; private set; }
        public static IEnumerable<IConfigurationInterceptor> ConfigurationInterceptors
        {
            get { return Interceptors.Where(x => x is IConfigurationInterceptor).Cast<IConfigurationInterceptor>(); }
        }
        public static IEnumerable<IConnectionStringInterceptor> ConnectionStringInterceptors
        {
            get { return Interceptors.Where(x => x is IConnectionStringInterceptor).Cast<IConnectionStringInterceptor>(); }
        }

        static ConfigurationManager()
        {
            Interceptors = new List<IInterceptor>();
        }

        public static void RegisterInterceptors(params IInterceptor[] interceptors)
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
            AppSettings = new AppSettingsExtended(appSettings, ConfigurationInterceptors);
            ConnectionStrings = new ConnectionStringsExtended(connectionStringSettings, AppSettings, ConnectionStringInterceptors);
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

        public Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap)
        {
            return System.Configuration.ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
        }
        
        /// <summary>
        /// Exists for in-place switching of System.Configuration.ConfigurationManager - avoid this static helper in new code
        /// </summary>
        public static IConfigurationManager Instance
        {
            get { return new ConfigurationManager(); }
        }
    }
}
