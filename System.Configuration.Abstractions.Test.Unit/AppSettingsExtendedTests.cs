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
            _fakeConfig.Add("my-string", "foobar");
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var val = wrapper["my-string"];

            Assert.That(val, Is.EqualTo("foobar"));
        }

        [Test]
        public void Indexer_WhenSettingDoesNotExist_ReturnsNull()
        {
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var val = wrapper["my-string"];

            Assert.That(val, Is.Null);
        }

        [Test]
        public void Setting_WhenValueExistsInConfiguration_ReturnsValueCorrectlyTyped()
        {
            _fakeConfig.Add("my-string", "foobar");
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var val = wrapper.AppSetting<string>("my-string");

            Assert.That(val, Is.EqualTo("foobar"));
        }

        [Test]
        public void Setting_WhenValueContainsKnownReplacementTokensAndTokensDontExist_ReturnsValueAsWritten()
        {
            _fakeConfig.Add("my-string", "{tenant}-{env}-{domain}-{team}-{hostedzone}");
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var val = wrapper.AppSetting<string>("my-string");

            Assert.That(val, Is.EqualTo("{tenant}-{env}-{domain}-{team}-{hostedzone}"));
        }

        [Test]
        public void Setting_RequestAnInt_ConvertsSettingValue()
        {
            _fakeConfig.Add("my-int", "123");
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var val = wrapper.AppSetting<int>("my-int");

            Assert.That(val, Is.EqualTo(123));
        }

        [TestCase("true", true)]
        [TestCase("True", true)]
        [TestCase("false", false)]
        [TestCase("False", false)]
        public void Setting_RequestABoolean_ConvertsSettingValue(string boolValue, bool expectation)
        {
            _fakeConfig.Add("my-bool", boolValue);
            var wrapper = new AppSettingsExtended(_fakeConfig);

            var val = wrapper.AppSetting<bool>("my-bool");

            Assert.That(val, Is.EqualTo(expectation));
        }
    }
}