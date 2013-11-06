using NUnit.Framework;

namespace System.Configuration.Abstractions.Test.Unit
{
    [TestFixture]
    public class ConfigurationManagerTests
    {
        #pragma warning disable 618
        [Test]
        public void Instance_IsNotNull_IsSingleton()
        {
            var instance1 = ConfigurationManager.Instance;
            var instance2 = ConfigurationManager.Instance;

            Assert.That(instance1, Is.EqualTo(instance2));
        }
        #pragma warning restore 618
    }
}