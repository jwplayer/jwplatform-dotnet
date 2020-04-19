using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace jwplatform
{
    public class Api
    {
        private readonly Client client;

        public Api(string apiKey, string apiSecret) : this(new Client(apiKey, apiSecret))
        { }

        internal Api(Client client)
        {
            this.client = client;
        }

        public JObject MakeGetRequest(string path, Dictionary<string, string> requestParams)
        {
            return GetResult(MakeGetRequestAsync(path, requestParams));
        }

        public Task<JObject> MakeGetRequestAsync(string path, Dictionary<string, string> requestParams)
        {
            return MakeRequest("GET", path, requestParams, false);
        }

        public JObject MakePostRequest(string path, Dictionary<string, string> requestParams, bool hasBodyParams)
        {
            return GetResult(MakePostRequestAsync(path, requestParams, hasBodyParams));
        }

        public Task<JObject> MakePostRequestAsync(string path, Dictionary<string, string> requestParams, bool hasBodyParams)
        {
            return MakeRequest("GET", path, requestParams, hasBodyParams);
        }

        public JObject Upload(Dictionary<string, string> videoInfo, string filePath)
        {
            return GetResult(UploadAsync(videoInfo, filePath));
        }

        public async Task<JObject> UploadAsync(Dictionary<string, string> videoInfo, string filePath)
        {
            var videosCreateResponse = await MakePostRequestAsync("videos/create", videoInfo, true);
            var link = videosCreateResponse["link"];
            var qs = link["query"].ToObject<Dictionary<string, string>>();

            var uploadUrl = link["protocol"] + "://" + link["address"] + link["path"] + "?api_format=json&key=" + qs["key"] + "&token=" + qs["token"];

            return ValidateUploadResponse(await client.UploadAsync(uploadUrl, filePath));
        }

        private Task<JObject> MakeRequest(
            string requestType,
            string path,
            Dictionary<string, string> requestParams,
            bool hasBodyParams)
        {

            if (requestType == null)
                throw new ArgumentNullException("Requst Type must be provided");

            if (path == null)
                throw new ArgumentNullException("Path must be provided");

            switch (requestType.ToUpper())
            {
                case "GET":
                    return GetAsyncRequest(path, requestParams);

                case "POST":
                    return PostAsyncRequest(path, requestParams, hasBodyParams);

                default:
                    throw new Exception("Request type not supported");
            }
        }

        private async Task<JObject> GetAsyncRequest(string path, Dictionary<string, string> requestParams)
        {
            var response = await client.GetAsync(path, requestParams);
            return await ValidateResponse(response);
        }

        private async Task<JObject> PostAsyncRequest(string path, Dictionary<string, string> requestParams, bool hasBodyParams)
        {
            var response = await client.PostAsync(path, requestParams, hasBodyParams);
            return await ValidateResponse(response);
        }

        private async Task<JObject> ValidateResponse(HttpResponseMessage response)
        {
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JObject.Parse(result);
            }
            else
            {
                throw new Exception("An error occurred. Status code: " + response.StatusCode + " - " + result);
            }
        }

        private JObject ValidateUploadResponse(string response)
        {
            var result = JObject.Parse(response);
            var status = result["status"].ToString();

            if (status == "ok")
            {
                return result;
            }
            else
            {
                throw new Exception("An error occurred. Status: " + status + " - " + response);
            }
        }

        private JObject GetResult(Task<JObject> asyncRequestTask)
        {
            return Task.Run(() => asyncRequestTask).GetAwaiter().GetResult();
        }
    }
}
