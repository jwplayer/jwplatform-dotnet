using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace jwplatform.tests
{
    internal static class MockClient
    {
        public static Client GetMockClient(string apiKey, string apiSecret)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                ).Returns((HttpRequestMessage request, CancellationToken cancelToken) =>
                    GetMockResponse(request, cancelToken));

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://test.com")
            };
            return new Client(apiKey, apiSecret, httpClient);
        }

        private static Task<HttpResponseMessage> GetMockResponse(HttpRequestMessage request, CancellationToken cancelToken)
        {
            if (request.RequestUri.LocalPath == "/videos/show")
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                response.Content = new StringContent(GetOkResponse(), Encoding.UTF8, "application/json");
                return Task.FromResult(response);
            }
            if (request.RequestUri.LocalPath == "/videos/delete")
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                response.Content = new StringContent(GetOkResponse(), Encoding.UTF8, "application/json");
                return Task.FromResult(response);
            }
            throw new NotImplementedException();
        }

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
