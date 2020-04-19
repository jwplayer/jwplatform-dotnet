using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace jwplatform.tests
{
    /// <summary>
    /// A mock client for unit testing.
    /// </summary>
    internal static class MockClient
    {

        /// <summary>
        /// Generates a Client with a mock HttpClient.
        /// </summary>
        /// <param name="apiKey"> An Api Key string to pass to the Client. </param>
        /// <param name="apiSecret"> An API Secret string to pass to the Client. </param>
        /// <returns> A Client containing a mock HttpClient. </returns>
        public static Client GetMockClient(string apiKey, string apiSecret)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                ).Returns((HttpRequestMessage request, CancellationToken cancelToken) =>
                    GetMockResponse(request));

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://test.com")
            };
            return new Client(apiKey, apiSecret, httpClient);
        }

        /// <summary>
        /// Creates a mock HttpRequestMessage for the request routes being unit tested.
        /// </summary>
        /// <param name="request"> An HttpRequestMessage from the request. </param>
        /// <param name="cancelToken"> A CancellationToken from the request. </param>
        /// <returns> The mock HttpResponseMessage. </returns>
        /// <exception cref="NotImplementedException">
        /// Thrown when trying to access a route not being unit tested.
        /// </exception>
        private static Task<HttpResponseMessage> GetMockResponse(HttpRequestMessage request)
        {
            if (request.RequestUri.LocalPath == "/videos/show")
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(GetOkResponse(), Encoding.UTF8, "application/json")
                };
                return Task.FromResult(response);
            }
            if (request.RequestUri.LocalPath == "/videos/delete")
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(GetOkResponse(), Encoding.UTF8, "application/json")
                };
                return Task.FromResult(response);
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a mock success response formatted as a JSON string.
        /// </summary>
        /// <returns> The mock response string. </returns>
        private static string GetOkResponse()
        {
            return @"{
                ""status"": {
                    ""message"": ""Ok"",
                    ""code"": 200
                }
            }";
        }
    }
}
