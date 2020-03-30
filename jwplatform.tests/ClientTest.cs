using System;
using Xunit;

namespace jwplatform.tests
{
    public class ClientTest
    {
        [Fact]
        public void Client_GivenNullApiKey_ThrowsArgumentNullException() {
            Action client = () => new Client(null, "apiSecret");
            Assert.Throws<ArgumentNullException>(client);
        }

        [Fact]
        public void Client_GivenNullApiSecret_ThrowsArgumentNullException() {
            Action client = () => new Client("apiKey", null);
            Assert.Throws<ArgumentNullException>(client);
        }

        [Fact]
        public void Client_GivenNullApiKeyAndSecret_ThrowsArgumentNullException() {
            Action client = () => new Client(null, null);
            Assert.Throws<ArgumentNullException>(client);
        }
    }
}
