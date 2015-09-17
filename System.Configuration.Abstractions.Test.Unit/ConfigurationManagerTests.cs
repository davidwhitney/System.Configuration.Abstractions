using System.Collections.Specialized;
using NUnit.Framework;

namespace System.Configuration.Abstractions.Test.Unit
{
    [TestFixture]
    public class ConfigurationManagerTests
    {
        [SetUp]
        public void SetUp()
        {
            ConfigurationManager.Interceptors.Clear();
        }

        #pragma warning disable 618
        [Test]
        public void Instance_ReturnsNewConfigManagerEveryTime()
        {
            var instance1 = ConfigurationManager.Instance;
            var instance2 = ConfigurationManager.Instance;

            Assert.That(instance1, Is.Not.EqualTo(instance2));
        }
        #pragma warning restore 618

        [Test]
        public void RegisterInterceptors_RegisterInterceptors_AddedToInterceptorsCollection()
        {
            var interceptor = new TestInterceptor();

            ConfigurationManager.RegisterInterceptors(interceptor);

            Assert.That(ConfigurationManager.Interceptors[0], Is.EqualTo(interceptor));
        }

        [Test]
        public void RegisterTypeConverters_GivenConverter_AddedToCollection()
        {
            var converter = new UserConverterExample();

            ConfigurationManager.RegisterTypeConverters(converter);

            Assert.That(ConfigurationManager.TypeConverters[0], Is.EqualTo(converter));
        }

        [Test]
        public void AppSetting_WithInterceptor_CallsInterceptor()
        {
            var interceptor = new TestInterceptor();
            ConfigurationManager.RegisterInterceptors(interceptor);
            var appSettings = new NameValueCollection {{"key", "value"}};
            var cfgMgr = new ConfigurationManager(appSettings);

            cfgMgr.AppSettings.AppSetting("key");

            Assert.That(interceptor.Called, Is.True);
        }

        private class TestInterceptor : IConfigurationInterceptor
        {
            public bool Called { get; set; }

            public string OnSettingRetrieve(IAppSettings appSettings, string key, string originalValue)
            {
                Called = true;
                return originalValue;
            }
        }
    }
}