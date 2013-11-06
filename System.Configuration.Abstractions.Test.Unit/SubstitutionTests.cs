using NUnit.Framework;

namespace System.Configuration.Abstractions.Test.Unit
{
    /*
    [TestFixture]
    public class SubstitutionTests
    {
        [Test]
        public void Setting_WhenValueContainsKnownReplacementTokens_ReturnsValueAfterReplacement()
        {
            _fakeConfig.Add("tenant", "tenant-here");
            _fakeConfig.Add("env", "environment-here");
            _fakeConfig.Add("domain", "domain-here");
            _fakeConfig.Add("team", "team-here");
            _fakeConfig.Add("hostedzone", "hostedzone-here");
            _fakeConfig.Add("my-string", "{tenant}-{env}-{domain}-{team}-{hostedzone}");
            _wrapper = new AppSettingsExtended(_fakeConfig);

            var val = _wrapper.AppSetting<string>("my-string");

            Assert.That(val, Is.EqualTo("tenant-here-environment-here-domain-here-team-here-hostedzone-here"));
        }
     
        [Test]
        public void Setting_WhenSubstitutionSettingAddedAtRuntime_CorrectlySubstitutes()
        {
            AppSettingsWrapper.AppSettingSubstitutions.Add("my-runtime-setting");

            _fakeConfig.Add("my-runtime-setting", "sub");
            _fakeConfig.Add("my-string", "{my-runtime-setting}");

            var val = _wrapper.AppSetting<string>("my-string");

            Assert.That(val, Is.EqualTo("sub"));
        }

        [Test]
        public void Setting_WhenValueDoesntExistInConfiguration_ThrowsExceptionWithMissingKeyInMessage()
        {
            const string key = "thing-that-totally-doesnt-exist";

            var ex = Assert.Throws<ConfigurationErrorsException>(() => _wrapper.AppSetting<string>(key));

            Assert.That(ex.Message, Is.StringContaining(key));
        }
    }*/
}