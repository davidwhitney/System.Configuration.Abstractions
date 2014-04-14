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
        public void MapSettings_WithMultipleTypes_Maps()
        {
            _underlyingConfiguration.Add("stringg", "some string");
            _underlyingConfiguration.Add("doublee", "123");

            var settingsDto = _wrapper.MapSettings<AppSettingsToType>();

            Assert.That(settingsDto.stringg, Is.EqualTo("some string"));
            Assert.That(settingsDto.doublee, Is.EqualTo(123m));
        }

        [Test]
        public void MapSettings_WithPropertyThatShouldBeMappedTwice_Maps()
        {
            _underlyingConfiguration.Add("stringg", "some string");

            var settingsDto = _wrapper.MapSettings<AppSettingsToType>();

            Assert.That(settingsDto.Stringg, Is.EqualTo("some string"));
            Assert.That(settingsDto.stringg, Is.EqualTo("some string"));
        }

        [Test]
        public void MapSettings_WithDotNotationValues_Maps()
        {
            _underlyingConfiguration.Add("SubDto.PropertyName", "some string");

            var settingsDto = _wrapper.MapSettings<AppSettingsToType>();

            Assert.That(settingsDto.SubDto.PropertyName, Is.EqualTo("some string"));
        }

        [Test]
        public void MapSettings_WithDotNotationValuesNestedDeeply_Maps()
        {
            _underlyingConfiguration.Add("SubDto.Deeper.PropertyName", "some string");

            var settingsDto = _wrapper.MapSettings<AppSettingsToType>();

            Assert.That(settingsDto.SubDto.Deeper.PropertyName, Is.EqualTo("some string"));
        }

        private class AppSettingsToType { public string Stringg { get; set; } public string stringg { get; set; } public double doublee { get; set; } public AppSettingsToTypeInner SubDto { get; set; } }
        private class AppSettingsToTypeInner { public string PropertyName { get; set; } public AppSettingsToTypeInner2 Deeper { get; set; } }
        private class AppSettingsToTypeInner2 { public string PropertyName { get; set; } }

        [Test]
        public void Setting_WhenValueDoesntExistInConfiguration_ThrowsExceptionWithMissingKeyInMessage()
        {
            const string key = "doesnt-exist";

            var ex = Assert.Throws<ConfigurationErrorsException>(() => _wrapper.AppSetting<string>(key));

            Assert.That(ex.Message, Is.StringContaining(key));
        }

        [Test]
        public void Count_UnderlyingCollectionHasAnItem_ReturnsCountOfUnderlyingConfigCollection()
        {
            _wrapper = new AppSettingsExtended(new NameValueCollection{{"test", "value"}});

            Assert.That(_wrapper.Count, Is.EqualTo(1));
        }

        [Test]
        public void Keys_UnderlyingCollectionHasAnItem_ReturnsKeysOfUnderlyingConfigCollection()
        {
            _wrapper = new AppSettingsExtended(new NameValueCollection{{"test", "value"}});

            Assert.That(_wrapper.Keys[0], Is.EqualTo("test"));
        }

        [Test]
        public void GetKey_WithConfigValue_ReturnsKeyFromUnderlyingCollection()
        {
            var key = _wrapper.GetKey(0);

            Assert.That(key, Is.EqualTo("key-here"));
        }

        [Test]
        public void Remove_WithConfigValue_RemovesUnderlyingItem()
        {
            _wrapper.Remove("key-here");

            Assert.That(_underlyingConfiguration, Is.Empty);
        }

        [Test]
        public void Set_WithConfigValue_SetsUnderlyingItem()
        {
            _wrapper.Set("key-here", "my value now");

            Assert.That(_underlyingConfiguration[0], Is.EqualTo("my value now"));
        }

        [Test]
        public void Clear_ClearsUnderlyingStore()
        {
            _underlyingConfiguration.Add("ah", "ha");

            _wrapper.Clear();

            Assert.That(_underlyingConfiguration, Is.Empty);
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