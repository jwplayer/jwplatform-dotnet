using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace jwplatform {
    public class Api {

        private readonly Client client;

        public Api(string apiKey, string apiSecret) {
            this.client = new Client(apiKey, apiSecret);
        }

        public JObject MakeGetRequest(string path, Dictionary<string, string> requestParams) {
            return MakeRequest("GET", path, requestParams, false);
        }

        public JObject MakePostRequest(string path, Dictionary<string, string> requestParams, bool hasBodyParams) {
            return MakeRequest("GET", path, requestParams, hasBodyParams);
        }

        /*
        * --- Parameters ---
        * requestType: GET by default
        * path: required
        * requestParams: can be null
        * hasBodyParams: false by default
        */
        public JObject MakeRequest(
            string requestType,
            string path,
            Dictionary<string, string> requestParams,
            bool hasBodyParams) {

            if (requestType == null)
                throw new ArgumentNullException("Requst Type must be provided");

            if (path == null)
                throw new ArgumentNullException("Path must be provided");

            switch(requestType.ToUpper()) {
                case "GET":
                    return GetResult(GetAsyncRequest(path, requestParams));

                case "POST":
                    return GetResult(PostAsyncRequest(path, requestParams, hasBodyParams));

                default:
                    throw new Exception("Request type not supported");
            }
        }

        public JObject Upload(Dictionary<string, string> videoOptions, string filePath) {
            var videosCreateResponse = Create(videoOptions);
            var link = videosCreateResponse["link"];
            var qs = link["query"].ToObject<Dictionary<string, string>>();

            var uploadUrl = link["protocol"] + "://" + link["address"] + link["path"] + "?api_format=json&key=" + qs["key"] + "&token=" + qs["token"];

            var uploadResponse = client.Upload(uploadUrl, filePath);
            return JObject.Parse(uploadResponse);
        }

        private JObject Create(Dictionary<string, string> videoData) {
            return MakeRequest("POST", "videos/create", videoData, true);
        }

        private async Task<JObject> GetAsyncRequest(string path, Dictionary<string, string> requestParams) {
            var response = client.GetAsync(path, requestParams).Result;
            return await ValidateResponse(response);
        }

        private async Task<JObject> PostAsyncRequest(string path, Dictionary<string, string> requestParams, bool hasBodyParams) {
            var response = await client.PostAsync(path, requestParams, hasBodyParams);
            return await ValidateResponse(response);
        }

        private async Task<JObject> ValidateResponse(HttpResponseMessage response) {
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode) {
                return JObject.Parse(result);
            }
            else {
                throw new Exception("An error occurred. Status code: " + response.StatusCode + " - " + result);
            }
        }

        private JObject GetResult(Task<JObject> asyncRequestTask) {
            return asyncRequestTask.GetAwaiter().GetResult();
        }
    }
}