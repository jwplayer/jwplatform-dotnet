using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace jwplatform.tests
{
    public class ApiTest
    {
        private readonly Api TestApi = new Api("testKey", "testSecret");

        [Fact]
        public void MakeRequest_GivenNullPath_ThrowsArgumentNullException()
        {
            Action makeRequest = () => TestApi.MakeGetRequest(null, null);
            Assert.Throws<ArgumentNullException>(makeRequest);
        }

        [Fact]
        public void MakeRequest_GivenNullRequestType_ThrowsArgumentNullException()
        {
            Action makeRequest = () => TestApi.MakeRequest(null, "path", null, false);
            Assert.Throws<ArgumentNullException>(makeRequest);        
        }

        [Fact]
        public void MakeRequest_GivenUnsupportedRequestType_ThrowsException()
        {
            Action makeRequest = () => TestApi.MakeRequest("Invalid", "path", null, false);
            Assert.Throws<Exception>(makeRequest);        
        }

        [Fact]
        public void MakeRequest_GivenInvalidPath_ThrowsException()
        {
            Func<JObject> makeRequest = () => TestApi.MakeGetRequest("Invalid", null);
            Assert.Throws<Exception>(makeRequest);        
        }

        [Fact]
        public void UploadFile_GivenNullFile_ThrowsException()
        {
            Func<JObject> uploadRequest = () => TestApi.Upload(new Dictionary<string, string>(), null);
            Assert.Throws<Exception>(uploadRequest);  
        }

        [Fact]
        public void UploadFile_GivenInvalidFile_ThrowsException()
        {
            Func<JObject> uploadRequest = () => TestApi.Upload(new Dictionary<string, string>(), "Invalid");
            Assert.Throws<Exception>(uploadRequest);
        }

        [Fact]
        public void UploadFile_GivenInvalidFileType_ThrowsException()
        {
            Func<JObject> uploadRequest = () => TestApi.Upload(new Dictionary<string, string>(), "../../../resources/Test.txt");
            Assert.Throws<Exception>(uploadRequest);
        }
    }
}