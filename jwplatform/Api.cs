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

        public Task<JObject> MakeRequest(string path) {
            return MakeRequest(path, null as Dictionary<string, string>);
        }

        public Task<JObject> MakeRequest(string requestType, string path) {
            return MakeRequest(requestType, path, null);
        }

        public Task<JObject> MakeRequest(string path, Dictionary<string, string> requestParams) {
            return MakeRequest("GET", path, requestParams);
        }

        public Task<JObject> MakeRequest(string requestType, string path, Dictionary<string, string> requestParams) {
            return MakeRequest(requestType, path, requestParams, false);
        }

        /*
        * --- Parameters ---
        * requestType: GET by default
        * path: required
        * requestParams: can be null
        * hasBodyParams: false by default
        */
        public Task<JObject> MakeRequest(
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
                    return GetAsyncRequest(path, requestParams);

                case "POST":
                    return PostAsyncRequest(path, requestParams, hasBodyParams);

                default:
                    throw new Exception("Request type not supported");
            }
        }

        public async Task<JObject> Upload(Dictionary<string, string> videoOptions, string filePath) {
            var videosCreateResponse = await Create(videoOptions);
            var link = videosCreateResponse["link"];
            var qs = link["query"].ToObject<Dictionary<string, string>>();

            var uploadUrl = link["protocol"] + "://" + link["address"] + link["path"] + "?api_format=json&key=" + qs["key"] + "&token=" + qs["token"];

            var uploadResponse = client.Upload(uploadUrl, filePath);
            return JObject.Parse(uploadResponse);
        }

        private Task<JObject> Create(Dictionary<string, string> videoData) {
            return MakeRequest("POST", "videos/create", videoData, true);
        }

        private async Task<JObject> GetAsyncRequest(string path, Dictionary<string, string> requestParams) {
            try {
                var response = await client.GetAsync(path, requestParams);
                return await ValidateResponse(response);
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        private async Task<JObject> PostAsyncRequest(string path, Dictionary<string, string> requestParams, bool hasBodyParams) {
            try {
                var response = await client.PostAsync(path, requestParams, hasBodyParams);
                return await ValidateResponse(response);
            }
            catch (Exception ex) {
                throw ex;
            }
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
    }
}