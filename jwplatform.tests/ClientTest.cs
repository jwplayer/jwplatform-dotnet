using System;
using Xunit;

namespace jwplatform.tests
{
    public class ClientTest
    {
        [Fact]
        public void Client_GivenApiKeyAndSecret_SetsUp()
        {
            new Client("testApiKey", "testApiSecret");
        }

        [Fact]
        public void Client_GivenNullApiKey_ThrowsArgumentNullException()
        {
            static void client() => new Client(null, "testApiSecret");
            Assert.Throws<ArgumentNullException>(client);
        }

        [Fact]
        public void Client_GivenNullApiSecret_ThrowsArgumentNullException()
        {
            static void client() => new Client("testApiKeys", null);
            Assert.Throws<ArgumentNullException>(client);
        }

        [Fact]
        public void Client_GivenNullApiKeyAndSecret_ThrowsArgumentNullException()
        {
            static void client() => new Client(null, null);
            Assert.Throws<ArgumentNullException>(client);
        }
    }
}
