using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;

namespace System.Configuration.Abstractions.Test.Unit
{
    public class AppSettingsExtendedTests
    {
        private NameValueCollection _underlyingConfiguration;
        private AppSettingsExtended _wrapper;

        [SetUp]
        public void SetUp()
        {
            _underlyingConfiguration = new NameValueCollection {{"key-here", "junk"}};
            _wrapper = new AppSettingsExtended(_underlyingConfiguration);
        }

        [Test]
        public void Indexer_WhenSettingExists_ReturnsSetting()
        {
            var val = _wrapper["key-here"];

            Assert.That(val, Is.EqualTo("junk"));
        }

        [Test]
        public void IndexerById_WhenSettingExists_ReturnsSetting()
        {
            var val = _wrapper[0];

            Assert.That(val, Is.EqualTo("junk"));
        }

        [Test]
        public void Get_WhenSettingExists_ReturnsSetting()
        {
            var val = _wrapper.Get("key-here");

            Assert.That(val, Is.EqualTo("junk"));
        }

        [Test]
        public void GetById_WhenSettingExists_ReturnsSetting()
        {
            var val = _wrapper.Get(0);

            Assert.That(val, Is.EqualTo("junk"));
        }

        [Test]
        public void Indexer_WhenSettingExists_RunsAnyRegisteredInterceptorsAndReturnsSetting()
        {
            _wrapper = new AppSettingsExtended(_underlyingConfiguration, new List<IConfigurationInterceptor>{new TestInterceptor("return this")});

            var val = _wrapper["key-here"];

            Assert.That(val, Is.EqualTo("return this"));
        }

        [Test]
        public void IndexerById_WhenSettingExists_RunsAnyRegisteredInterceptorsAndReturnsSetting()
        {
            _wrapper = new AppSettingsExtended(_underlyingConfiguration, new List<IConfigurationInterceptor>{new TestInterceptor("return this")});

            var val = _wrapper[0];

            Assert.That(val, Is.EqualTo("return this"));
        }

        [Test]
        public void Get_WhenSettingExistsAndInterceptorPresent_ReturnsSetting()
        {
            _wrapper = new AppSettingsExtended(_underlyingConfiguration, new List<IConfigurationInterceptor> { new TestInterceptor("return this") });
            var val = _wrapper.Get("key-here");

            Assert.That(val, Is.EqualTo("return this"));
        }

        [Test]
        public void GetById_WhenSettingExistsAndInterceptorPresent_ReturnsSetting()
        {
            _wrapper = new AppSettingsExtended(_underlyingConfiguration, new List<IConfigurationInterceptor> { new TestInterceptor("return this") });
            
            var val = _wrapper.Get(0);

            Assert.That(val, Is.EqualTo("return this"));
        }

        [Test]
        public void Indexer_WhenSettingDoesNotExist_ReturnsNull()
        {
            _underlyingConfiguration.Clear();

            var val = _wrapper["key-here"];

            Assert.That(val, Is.Null);
        }

        [Test]
        public void Indexer_WhenSettingDoesNotExist_AndInterceptorPresentReturnsNull()
        {
            _underlyingConfiguration.Clear();
            _wrapper = new AppSettingsExtended(_underlyingConfiguration, new List<IConfigurationInterceptor> { new NullInterceptor() });

            var val = _wrapper["key-here"];

            Assert.That(val, Is.Null);
        }

        [Test]
        public void Setting_WhenSettingDoesNotExistAndActionSupplied_PerformsAction()
        {
            var val = _wrapper.AppSetting<string>("key-that-doesnt-exist", () => "default thing");

            Assert.That(val, Is.EqualTo("default thing"));
        }

        [Test]
        public void Setting_WhenSettingDoesNotExistAndActionSuppliedReturnsATypedThing_ReturnsTypedThing()
        {
            var val = _wrapper.AppSetting("key-that-doesnt-exist", () => 1);

            Assert.That(val, Is.EqualTo(1));
        }

        [Test]
        public void Setting_WhenSettingDoesNotExistAndActionSuppliedThrows_Throws()
        {
            Assert.Throws<InvalidOperationException>(
                () => _wrapper.AppSetting<string>("key-that-doesnt-exist", () => { throw new InvalidOperationException("HA!"); }));
        }

        [Test]
        public void Setting_WhenValueExistsInConfiguration_ReturnsValueCorrectlyTyped()
        {
            var val = _wrapper.AppSetting<string>("key-here");

            Assert.That(val, Is.EqualTo("junk"));
        }

        [Test]
        public void Setting_RequestAnInt_ConvertsSettingValue()
        {
            _underlyingConfiguration.Add("int", "123");

            var val = _wrapper.AppSetting<int>("int");

            Assert.That(val, Is.EqualTo(123));
        }

        [TestCase("true", true)]
        [TestCase("True", true)]
        [TestCase("false", false)]
        [TestCase("False", false)]
        public void Setting_RequestABoolean_ConvertsSettingValue(string boolValue, bool expectation)
        {
            _underlyingConfiguration.Add("boolean", boolValue);

            var val = _wrapper.AppSetting<bool>("boolean");

            Assert.That(val, Is.EqualTo(expectation));
        }

        [Test]
        public void Setting_RequestADouble_ConvertsSettingValue()
        {
            _underlyingConfiguration.Add("double", "1.0");

            var val = _wrapper.AppSetting<double>("double");

            Assert.That(val, Is.EqualTo(1.0m));
        }

        [Test]
        public void Setting_RequestAnInvalidDouble_ThrowsUnderlyingException()
        {
            _underlyingConfiguration.Add("double", "NO DOUBLE HERE");

            Assert.Throws<FormatException>(() => _wrapper.AppSetting<double>("double"));
        }

        [Test]
        public void Setting_RequestAnInvalidDoubleWithDefault_ThrowsUnderlyingException()
        {
            _underlyingConfiguration.Add("double", "NO DOUBLE HERE");

            Assert.Throws<FormatException>(() => _wrapper.AppSetting("double", () => 1.0));
        }

        [Test]
        public void Setting_WhenValueDoesntExistInConfiguration_ThrowsExceptionWithMissingKeyInMessage()
        {
            const string key = "doesnt-exist";

            var ex = Assert.Throws<ConfigurationErrorsException>(() => _wrapper.AppSetting<string>(key));

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