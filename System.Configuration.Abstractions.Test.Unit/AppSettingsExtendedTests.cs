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
        public void Ctor_WhenPassedNullSettings_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new AppSettingsExtended(null));
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
        public void Indexer_WhenSettingDoesNotExist_ReturnsNull()
        {
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var val = wrapper["key-here"];

            Assert.That(val, Is.Null);
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
        public void Setting_WhenValueDoesntExistInConfiguration_ThrowsExceptionWithMissingKeyInMessage()
        {
            const string key = "doesnt-exist";
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var ex = Assert.Throws<ConfigurationErrorsException>(() => wrapper.AppSetting<string>(key));

            Assert.That(ex.Message, Is.StringContaining(key));
        }
    }
}