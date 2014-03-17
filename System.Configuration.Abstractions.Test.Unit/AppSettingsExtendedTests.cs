using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;

namespace System.Configuration.Abstractions.Test.Unit
{
    public class AppSettingsExtendedTests
    {
        private NameValueCollection _fakeConfig;

        [SetUp]
        public void SetUp()
        {
            _fakeConfig = new NameValueCollection();
        }

        [Test]
        public void Indexer_WhenSettingExists_ReturnsSetting()
        {
            _fakeConfig.Add("key-here", "junk");
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var val = wrapper["key-here"];

            Assert.That(val, Is.EqualTo("junk"));
        }

        [Test]
        public void Indexer_WhenSettingExists_RunsAnyRegisteredInterceptorsAndReturnsSetting()
        {
            _fakeConfig.Add("key-here", "junk");
            var wrapper = new AppSettingsExtended(_fakeConfig, new List<IConfigurationInterceptor>{new TestInterceptor("return this")});

            var val = wrapper["key-here"];

            Assert.That(val, Is.EqualTo("return this"));
        }

        [Test]
        public void Indexer_WhenSettingDoesNotExist_ReturnsNull()
        {
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var val = wrapper["key-here"];

            Assert.That(val, Is.Null);
        }

        [Test]
        public void Indexer_WhenSettingDoesNotExist_AndInterceptorPresentReturnsNull()
        {
            var wrapper = new AppSettingsExtended(_fakeConfig, new List<IConfigurationInterceptor> { new NullInterceptor() });

            var val = wrapper["key-here"];

            Assert.That(val, Is.Null);
        }

        [Test]
        public void Setting_WhenSettingDoesNotExistAndActionSupplied_PerformsAction()
        {
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var val = wrapper.AppSetting<string>("string", () => "default thing");

            Assert.That(val, Is.EqualTo("default thing"));
        }

        [Test]
        public void Setting_WhenSettingDoesNotExistAndActionSuppliedReturnsATypedThing_ReturnsTypedThing()
        {
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var val = wrapper.AppSetting<int>("string", () => 1);

            Assert.That(val, Is.EqualTo(1));
        }

        [Test]
        public void Setting_WhenSettingDoesNotExistAndActionSuppliedThrows_Throws()
        {
            var wrapper = new AppSettingsExtended(_fakeConfig);

            Assert.Throws<InvalidOperationException>(
                () => wrapper.AppSetting<string>("string", () => { throw new InvalidOperationException("HA!"); }));
        }

        [Test]
        public void Setting_WhenValueExistsInConfiguration_ReturnsValueCorrectlyTyped()
        {
            _fakeConfig.Add("string", "junk");
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var val = wrapper.AppSetting<string>("string");

            Assert.That(val, Is.EqualTo("junk"));
        }

        [Test]
        public void Setting_RequestAnInt_ConvertsSettingValue()
        {
            _fakeConfig.Add("int", "123");
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var val = wrapper.AppSetting<int>("int");

            Assert.That(val, Is.EqualTo(123));
        }

        [TestCase("true", true)]
        [TestCase("True", true)]
        [TestCase("false", false)]
        [TestCase("False", false)]
        public void Setting_RequestABoolean_ConvertsSettingValue(string boolValue, bool expectation)
        {
            _fakeConfig.Add("boolean", boolValue);
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var val = wrapper.AppSetting<bool>("boolean");

            Assert.That(val, Is.EqualTo(expectation));
        }

        [Test]
        public void Setting_RequestADouble_ConvertsSettingValue()
        {
            _fakeConfig.Add("double", "1.0");
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var val = wrapper.AppSetting<double>("double");

            Assert.That(val, Is.EqualTo(1.0m));
        }

        [Test]
        public void Setting_RequestAnInvalidDouble_ThrowsUnderlyingException()
        {
            _fakeConfig.Add("double", "NO DOUBLE HERE");
            var wrapper = new AppSettingsExtended(_fakeConfig);

            Assert.Throws<FormatException>(() => wrapper.AppSetting<double>("double"));
        }

        [Test]
        public void Setting_RequestAnInvalidDoubleWithDefault_ThrowsUnderlyingException()
        {
            _fakeConfig.Add("double", "NO DOUBLE HERE");
            var wrapper = new AppSettingsExtended(_fakeConfig);

            Assert.Throws<FormatException>(() => wrapper.AppSetting("double", () => 1.0));
        }

        [Test]
        public void Setting_WhenValueDoesntExistInConfiguration_ThrowsExceptionWithMissingKeyInMessage()
        {
            const string key = "doesnt-exist";
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var ex = Assert.Throws<ConfigurationErrorsException>(() => wrapper.AppSetting<string>(key));

            Assert.That(ex.Message, Is.StringContaining(key));
        }
    }

    public class TestInterceptor : IConfigurationInterceptor
    {
        private readonly string _returnThis;

        public TestInterceptor(string returnThis)
        {
            _returnThis = returnThis;
        }

        public string OnSettingRetrieve(IAppSettings appSettings, string key, string originalValue)
        {
            return _returnThis;
        }
    }

    public class NullInterceptor : IConfigurationInterceptor
    {
        public string OnSettingRetrieve(IAppSettings appSettings, string key, string originalValue)
        {
            return originalValue;
        }
    }
}