using NUnit.Framework;
using Barbershop.Utils;

namespace Barbershop.Tests.Utils
{
    public class SecurityUtilsTests
    {
        [Test]
        public void Hash_ValidInput_ReturnsHash()
        {
            var hash = SecurityUtils.Hash("test");
            Assert.IsNotEmpty(hash);
        }

        [Test]
        public void Hash_NullInput_ReturnsEmpty()
        {
            var hash = SecurityUtils.Hash(null);
            Assert.AreEqual(string.Empty, hash);
        }
    }
}