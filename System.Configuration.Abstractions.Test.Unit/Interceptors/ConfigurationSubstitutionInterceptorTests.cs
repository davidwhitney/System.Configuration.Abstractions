using System.Collections.Specialized;
using System.Configuration.Abstractions.Interceptors;
using NUnit.Framework;

namespace System.Configuration.Abstractions.Test.Unit.Interceptors
{
    [TestFixture]
    public class ConfigurationSubstitutionInterceptorTests
    {
        private NameValueCollection _underlyingAppSettingStore;
        private AppSettingsExtended _appSettings;
        private ConfigurationSubstitutionInterceptor _interceptor;
        private ConnectionStringSettingsCollection _connectionStrings;
        private ConnectionStringsExtended _connectionStringsExtended;

        [SetUp]
        public void SetUp()
        {
            _underlyingAppSettingStore = new NameValueCollection();
            _connectionStrings = new ConnectionStringSettingsCollection();
            _appSettings = new AppSettingsExtended(_underlyingAppSettingStore);
            _interceptor = new ConfigurationSubstitutionInterceptor();
            _connectionStringsExtended = new ConnectionStringsExtended(_connectionStrings, _appSettings, new[] {_interceptor});
        }

        [Test]
        public void OnSettingRetrieve_WhenValueContainsReplacementTokensOfOtherConfigKey_ReturnsValueAfterReplacement()
        {
            _appSettings.Add("tenant", "tenant-here");

            var val = _interceptor.OnSettingRetrieve(_appSettings, "key", "{tenant}");

            Assert.That(val, Is.EqualTo("tenant-here"));
        }

        [Test]
        public void OnSettingRetrieve_WhenValueContainsReplacementTokensAndKeyDoesntExist_ReturnsValueAsWritten()
        {
            var val = _interceptor.OnSettingRetrieve(_appSettings, "key", "{tenant}");

            Assert.That(val, Is.EqualTo("{tenant}"));
        }

        [Test]
        public void OnConnectionStringRetrieve_WhenValueContainsReplacementTokensOfOtherConfigKey_ReturnsValueAfterReplacement()
        {
            _appSettings.Add("tenant", "tenant-here");
            var connString = new ConnectionStringSettings("name", "{tenant}");

            var val = _interceptor.OnConnectionStringRetrieve(_appSettings, _connectionStringsExtended, connString);

            Assert.That(val.ConnectionString, Is.EqualTo("tenant-here"));
        }

        [Test]
        public void OnConnectionStringRetrieve_WhenValueContainsReplacementTokensAndKeyDoesntExist_ReturnsValueAsWritten()
        {
            var connString = new ConnectionStringSettings("name", "{tenant}");

            var val = _interceptor.OnConnectionStringRetrieve(_appSettings, _connectionStringsExtended, connString);

            Assert.That(val.ConnectionString, Is.EqualTo("{tenant}"));
        }
    }
}