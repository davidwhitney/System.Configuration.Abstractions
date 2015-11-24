using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;

namespace System.Configuration.Abstractions.Test.Unit
{
    [TestFixture]
    public class ConnectionStringsExtendedTests
    {
        private ConnectionStringSettingsCollection _fakeConfig;
        private ConnectionStringsExtended _wrapper;

        [SetUp]
        public void SetUp()
        {
            _fakeConfig = new ConnectionStringSettingsCollection { new ConnectionStringSettings("key-here", "junk") };
            _wrapper = new ConnectionStringsExtended(_fakeConfig, new AppSettingsExtended(new NameValueCollection()));
        }

        [Test]
        public void Indexer_WhenSettingExists_ReturnsSetting()
        {
            var val = _wrapper["key-here"];

            Assert.That(val.ConnectionString, Is.EqualTo("junk"));
        }

        [Test]
        public void Indexer_WhenSettingExists_RunsAnyRegisteredInterceptorsAndReturnsSetting()
        {
            var wrapper = new ConnectionStringsExtended(_fakeConfig, new AppSettingsExtended(new NameValueCollection()),
                    new List<IConnectionStringInterceptor> { new TestConnectionStringInterceptor("return this") });

            var val = wrapper["key-here"];

            Assert.That(val.ConnectionString, Is.EqualTo("return this"));
        }

        [Test]
        public void Indexer_WhenSettingDoesNotExist_ReturnsNull()
        {
            var val = _wrapper["key-not-here"];

            Assert.That(val, Is.Null);
        }

        [Test]
        public void Indexer_WhenSettingDoesNotExist_AndInterceptorPresentReturnsNull()
        {
            var wrapper = new ConnectionStringsExtended(_fakeConfig, new AppSettingsExtended(new NameValueCollection()),
                    new List<IConnectionStringInterceptor> { new NullConnectionStringInterceptor() });

            var val = wrapper["key-here"];

            Assert.That(val, Is.Null);
        }

        [Test]
        public void ConnectionStrings_Are_Enumerable()
        {
            _wrapper.Add(new ConnectionStringSettings("UK_Conn", "UK_DB_ConnString"));

            Assert.That(_wrapper.GetEnumerator(), Is.Not.Null);
            Assert.That(_wrapper.Raw.Count, Is.EqualTo(2));
        }

        public class NullConnectionStringInterceptor : IConnectionStringInterceptor
        {
            public ConnectionStringSettings OnConnectionStringRetrieve(IAppSettings appSettings, IConnectionStrings connectionStrings,
                ConnectionStringSettings originalValue)
            {
                return null;
            }
        }

        public class TestConnectionStringInterceptor : IConnectionStringInterceptor
        {
            private readonly string _returnThis;

            public TestConnectionStringInterceptor(string returnThis)
            {
                _returnThis = returnThis;
            }

            public ConnectionStringSettings OnConnectionStringRetrieve(IAppSettings appSettings, IConnectionStrings connectionStrings,
                ConnectionStringSettings originalValue)
            {
                return new ConnectionStringSettings(originalValue.Name, _returnThis, originalValue.ProviderName);
            }
        }
    }
}