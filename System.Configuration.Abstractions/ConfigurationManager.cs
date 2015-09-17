using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Abstractions.TypeConverters;
using System.Linq;

namespace System.Configuration.Abstractions
{
    public class ConfigurationManager : IConfigurationManager
    {
        public IAppSettings AppSettings { get; set; }
        public IConnectionStrings ConnectionStrings { get; set; }

        public static List<IConvertType> TypeConverters { get; private set; }
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
            TypeConverters = new List<IConvertType>();
        }

        public static void RegisterInterceptors(params IInterceptor[] interceptors)
        {
            Interceptors.AddRange(interceptors);
        }

        public static void RegisterTypeConverters(params IConvertType[] converters)
        {
            TypeConverters.AddRange(converters);
        }

        public ConfigurationManager() :
            this(System.Configuration.ConfigurationManager.AppSettings,
                System.Configuration.ConfigurationManager.ConnectionStrings)
        {
        }

        public ConfigurationManager(NameValueCollection appSettings)
            : this(appSettings, System.Configuration.ConfigurationManager.ConnectionStrings)
        {
        }

        public ConfigurationManager(NameValueCollection appSettings, ConnectionStringSettingsCollection connectionStringSettings)
        {
            AppSettings = new AppSettingsExtended(appSettings, ConfigurationInterceptors, ConfigureDefaultTypeConverters());
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

#if vLatest
        public Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap, ConfigurationUserLevel userLevel, bool preLoad)
        {
            return System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, userLevel, preLoad);
        }

#endif

        /// <summary>
        /// Exists for in-place switching of System.Configuration.ConfigurationManager - avoid this static helper in new code
        /// </summary>
        public static IConfigurationManager Instance
        {
            get { return new ConfigurationManager(); }
        }

        private static IEnumerable<IConvertType> ConfigureDefaultTypeConverters()
        {
            // Defaults
            var typeConverters = new List<IConvertType>
            {
                new UriConverter()
            };

            // User supplied
            typeConverters.AddRange(TypeConverters);
            return typeConverters;
        }
    }
}
