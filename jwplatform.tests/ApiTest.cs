using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xunit;

namespace jwplatform.tests
{
    public class ApiTest
    {
        private readonly Api TestApi = new Api("testKey", "testSecret");

        [Fact]
        public void MakeGetRequest_GivenNullPath_ThrowsArgumentNullException()
        {
            void makeRequest() => TestApi.GetRequest(null, null);
            Assert.Throws<ArgumentNullException>(makeRequest);
        }

        [Fact]
        public void MakeGetRequest_GivenInvalidPath_ThrowsException()
        {
            JObject makeRequest() => TestApi.GetRequest("Invalid", null);
            Assert.Throws<Exception>(makeRequest);
        }

        [Fact]
        public void MakeGetRequest_GivenValidVideoShowPath_CompletesSuccessfully()
        {
            var mockClient = MockClient.GetMockClient("testKey", "testSecret");
            var mockApi = new Api(mockClient);
            var requestParams = new Dictionary<string, string> {
                {"video_key", "MEDIA_ID"}
            };
            var result = mockApi.GetRequest("/videos/show", requestParams);
            Assert.Equal("Ok", result["status"]["message"]);
            Assert.Equal(200, result["status"]["code"]);
        }

        [Fact]
        public async void MakeGetRequestAsync_GivenSpecialCharacters_CompletesSuccessfully()
        {
            var mockClient = MockClient.GetMockClient("testKey", "testSecret");
            var mockApi = new Api(mockClient);
            var requestParams = new Dictionary<string, string> {
                {"video_key", "MEDIA_ID"},
                {"special_characters", "te$t media&*"}
            };
            var result = await mockApi.GetRequestAsync("/videos/show", requestParams);
            Assert.Equal("Ok", result["status"]["message"]);
            Assert.Equal(200, result["status"]["code"]);
        }

        [Fact]
        public void MakePostRequest_GivenNullPath_ThrowsArgumentNullException()
        {
            void makeRequest() => TestApi.PostRequest(null, new Dictionary<string, string>(), false);
            Assert.Throws<ArgumentNullException>(makeRequest);
        }

        [Fact]
        public void MakePostRequest_GivenInvalidPath_ThrowsException()
        {
            JObject makeRequest() => TestApi.PostRequest("Invalid", new Dictionary<string, string>(), false);
            Assert.Throws<Exception>(makeRequest);
        }

        [Fact]
        public void MakePostRequest_GivenValidDeletePath_CompletesSuccessfully()
        {
            var mockClient = MockClient.GetMockClient("testKey", "testSecret");
            var mockApi = new Api(mockClient);
            var requestParams = new Dictionary<string, string> {
                {"video_key", "MEDIA_ID"},
            };
            var result = mockApi.PostRequest("/videos/delete", requestParams, true);

            Assert.Equal("Ok", result["status"]["message"]);
            Assert.Equal(200, result["status"]["code"]);
        }

        [Fact]
        public async void MakePostRequestAsync_GivenSpecialCharacters_CompletesSuccessfully()
        {
            var mockClient = MockClient.GetMockClient("testKey", "testSecret");
            var mockApi = new Api(mockClient);
            var requestParams = new Dictionary<string, string> {
                {"video_key", "MEDIA_ID"},
                {"special_characters", "te$t media&*"}
            };
            var result = await mockApi.PostRequestAsync("/videos/delete", requestParams, true);
            Assert.Equal("Ok", result["status"]["message"]);
            Assert.Equal(200, result["status"]["code"]);
        }
    }
}
