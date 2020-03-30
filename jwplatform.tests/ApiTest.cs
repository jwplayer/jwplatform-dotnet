using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace jwplatform.tests
{
    public class ApiTest
    {
        private readonly Api TestApi = new Api("testKey", "testSecret");

        [Fact]
        public void MakeRequest_GivenNullPath_ThrowsArgumentNullException()
        {
            Action makeRequest = () => TestApi.MakeRequest(null);
            Assert.Throws<ArgumentNullException>(makeRequest);
        }

        [Fact]
        public void MakeRequest_GivenNullRequestType_ThrowsArgumentNullException()
        {
            Action makeRequest = () => TestApi.MakeRequest(null, "path");
            Assert.Throws<ArgumentNullException>(makeRequest);        
        }

        [Fact]
        public void MakeRequest_GivenUnsupportedRequestType_ThrowsException()
        {
            Action makeRequest = () => TestApi.MakeRequest("Invalid", "path");
            Assert.Throws<Exception>(makeRequest);        
        }

        [Fact]
        public async void MakeRequest_GivenInvalidPath_ThrowsException()
        {
            Task makeRequest() => TestApi.MakeRequest("GET", "Invalid");
            await Assert.ThrowsAsync<Exception>(makeRequest);        
        }

        [Fact]
        public async void UploadFile_GivenNullFile_ThrowsException()
        {
            Task uploadRequest() => TestApi.Upload(new Dictionary<string, string>(), null);
            await Assert.ThrowsAsync<Exception>(uploadRequest);  
        }

        [Fact]
        public async void UploadFile_GivenInvalidFile_ThrowsException()
        {
            Task uploadRequest() => TestApi.Upload(new Dictionary<string, string>(), "Invalid");
            await Assert.ThrowsAsync<Exception>(uploadRequest);
        }

        [Fact]
        public async void UploadFile_GivenInvalidFileType_ThrowsException()
        {
            Task uploadRequest() => TestApi.Upload(new Dictionary<string, string>(), "../../../resources/Test.txt");
            await Assert.ThrowsAsync<Exception>(uploadRequest);
        }
    }
}