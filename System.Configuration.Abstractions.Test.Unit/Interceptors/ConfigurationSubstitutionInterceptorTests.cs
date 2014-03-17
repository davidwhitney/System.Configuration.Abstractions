using System.Collections.Specialized;
using System.Configuration.Abstractions.Interceptors;
using NUnit.Framework;

namespace System.Configuration.Abstractions.Test.Unit.Interceptors
{
    [TestFixture]
    public class ConfigurationSubstitutionInterceptorTests
    {
        private NameValueCollection _nvc;
        private AppSettingsExtended _fakeConfig;
        private ConfigurationSubstitutionInterceptor _wrapper;

        [SetUp]
        public void SetUp()
        {
            _nvc = new NameValueCollection();
            _fakeConfig = new AppSettingsExtended(_nvc);
            _wrapper = new ConfigurationSubstitutionInterceptor();
        }

        [Test]
        public void OnSettingRetrieve_WhenValueContainsReplacementTokensOfOtherConfigKey_ReturnsValueAfterReplacement()
        {
            _fakeConfig.Add("tenant", "tenant-here");

            var val = _wrapper.OnSettingRetrieve(_fakeConfig, "key", "{tenant}");

            Assert.That(val, Is.EqualTo("tenant-here"));
        }

        [Test]
        public void OnSettingRetrieve_WhenValueContainsReplacementTokensAndKeyDoesntExist_ReturnsValueAsWritten()
        {
            var val = _wrapper.OnSettingRetrieve(_fakeConfig, "key", "{tenant}");

            Assert.That(val, Is.EqualTo("{tenant}"));
        }
    }
}