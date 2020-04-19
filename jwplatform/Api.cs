using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace jwplatform
{
    /// <summary>
    /// Used to make JWPlatform API calls.
    /// </summary>
    /// <see href="https://github.com/jwplayer/jwplatform-dotnet">
    /// JWPlatform Dotnet GitHub
    /// </see>
    public class Api
    {
        private readonly Client client;

        /// <summary>
        /// Constructor to instantiate Api with a <see cref="Client" /> to fulfill requests.
        /// </summary>
        /// <param name="apiKey"> A JWPlatform API Key. </param>
        /// <param name="apiSecret"> A JWPlatform API Secret. </param>
        public Api(string apiKey, string apiSecret) : this(new Client(apiKey, apiSecret))
        { }

        /// <summary>
        /// Constructor that takes in a <see cref="Client" /> used for unit testing.
        /// </summary>
        /// <param name="client"> A <see cref="Client" /> used to fulfill requests. </param>
        internal Api(Client client)
        {
            this.client = client;
        }

        /// <summary>
        /// The synchronous version of <see cref="Api.MakeGetRequestAsync(string, Dictionary{string, string})" />.
        /// </summary>
        public JObject MakeGetRequest(string path, Dictionary<string, string> requestParams)
        {
            return GetResult(MakeGetRequestAsync(path, requestParams));
        }

        /// <summary>
        /// Executes a GET API request with <see cref="Api.MakeRequest(string, string, Dictionary{string, string}, bool)" />.
        /// </summary>
        /// <param name="requestPath"> A string representing the request path. </param>
        /// <param name="requestParams">
        /// A Dictionary of string keys and values of the request parameters. Set to null if there are none.
        /// </param>
        /// <returns> A JSON Object response. </returns>
        public Task<JObject> MakeGetRequestAsync(string requestPath, Dictionary<string, string> requestParams)
        {
            return MakeRequest("GET", requestPath, requestParams, false);
        }

        /// <summary>
        /// The synchronous version of <see cref="Api.MakePostRequestAsync(string, Dictionary{string, string}, bool)" />.
        /// </summary>
        public JObject MakePostRequest(string path, Dictionary<string, string> requestParams, bool hasBodyParams)
        {
            return GetResult(MakePostRequestAsync(path, requestParams, hasBodyParams));
        }

        /// <summary>
        /// Executes a POST API request with <see cref="Api.MakeRequest(string, string, Dictionary{string, string}, bool)" />.
        /// </summary>
        /// <param name="requestPath"> A string representing the request path. </param>
        /// <param name="requestParams">
        /// A Dictionary of string keys and values of the request parameters. Set to null if there are none.
        /// </param>
        /// <param name="hasBodyParams">
        /// A boolean indicating whether the parameters are body or query parameters.
        /// </param>
        /// <returns> A JSON Object response. </returns>
        public Task<JObject> MakePostRequestAsync(string requestPath, Dictionary<string, string> requestParams, bool hasBodyParams)
        {
            return MakeRequest("POST", requestPath, requestParams, hasBodyParams);
        }

        /// <summary>
        /// The synchronous version of <see cref="Api.UploadAsync(Dictionary{string, string}, string)" />.
        /// </summary>
        public JObject Upload(Dictionary<string, string> videoInfo, string filePath)
        {
            return GetResult(UploadAsync(videoInfo, filePath));
        }

        /// <summary>
        /// Used to upload a single video file. First a video is created via a POST request. An upload URL is
        /// constructed with the video created response. The URL is then used to invoke the 
        /// <see cref="Client.UploadAsync(string, string)" />.
        /// </summary>
        /// <param name="videoInfo">
        /// A Dictionary of string keys and values with information about the file such as author, title, category, etc.
        /// </param>
        /// <param name="filePath"> A string representing the local path to the video file. </param>
        /// <returns> A JSON Object response. </returns>
        public async Task<JObject> UploadAsync(Dictionary<string, string> videoInfo, string filePath)
        {
            var videosCreateResponse = await MakePostRequestAsync("videos/create", videoInfo, true);
            var link = videosCreateResponse["link"];
            var qs = link["query"].ToObject<Dictionary<string, string>>();

            var uploadUrl = link["protocol"] + "://" + link["address"] + link["path"] + 
                "?api_format=json&key=" + qs["key"] + "&token=" + qs["token"];

            return ValidateUploadResponse(await client.UploadAsync(uploadUrl, filePath));
        }

        /// <summary>
        /// General method called by <see cref="Api.MakeGetRequestAsync(string, Dictionary{string, string})" /> and
        /// <see cref="Api.MakePostRequestAsync(string, Dictionary{string, string}, bool)" />.
        /// </summary>
        /// <param name="requestType"> A string indicating the request tye (GET or POST). </param>
        /// <param name="requestPath"> A string representing the request path. </param>
        /// <param name="requestParams"> A Dictionary of string keys and values of the request parameters. </param>
        /// <param name="hasBodyParams">
        /// A boolean flag indicating if the request parameters are body parameters (true)
        /// or query string parameters (false).
        /// </param>
        /// <returns> A JSON object of the request response. </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the request path is null.
        /// </exception>
        private Task<JObject> MakeRequest(
            string requestType,
            string requestPath,
            Dictionary<string, string> requestParams,
            bool hasBodyParams)
        {
            if (requestPath == null)
                throw new ArgumentNullException("Path must be provided");

            switch (requestType.ToUpper())
            {
                case "GET":
                    return GetAsyncRequest(requestPath, requestParams);

                default:
                    return PostAsyncRequest(requestPath, requestParams, hasBodyParams);
            }
        }

        /// <summary>
        /// Calls the Client's GetAsync method and validates it's response.
        /// </summary>
        /// <see cref="Client.GetAsync(string, Dictionary{string, string})"/>
        /// <returns> The JSON response object. </returns>
        private async Task<JObject> GetAsyncRequest(string requestPath, Dictionary<string, string> requestParams)
        {
            var response = await client.GetAsync(requestPath, requestParams);
            return await ValidateResponse(response);
        }

        /// <summary>
        /// Calls the Client's PostAsync method and validates it's response.
        /// </summary>
        /// <see cref="Client.PostAsync(string, Dictionary{string, string}, bool)"/>
        /// <returns> The JSON response object. </returns>
        private async Task<JObject> PostAsyncRequest(string requestPath, Dictionary<string, string> requestParams, bool hasBodyParams)
        {
            var response = await client.PostAsync(requestPath, requestParams, hasBodyParams);
            return await ValidateResponse(response);
        }

        /// <summary>
        /// Validates Client responses and returns if a request was successful.
        /// </summary>
        /// <param name="response"> An HttpResponseMessage. </param>
        /// <returns> A JSON object of the <paramref name="response"/> content. </returns>
        /// <exception cref="Exception">
        /// Thrown when the response contains an unsuccessful status code.
        /// </exception>
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

        /// <summary>
        /// Validates Client upload responses and returns if a request was successful.
        /// </summary>
        /// <param name="response"> An string representing a request response. </param>
        /// <returns> A JSON object of the <paramref name="response"/>. </returns>
        /// <exception cref="Exception">
        /// Thrown when the response contains an unsuccessful status.
        /// </exception>
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

        /// <summary>
        /// Runs an asynchronous API task synchronously and returns the JSON object result.
        /// </summary>
        /// <param name="asyncRequestTask">
        /// An asynchronous task that returns a JSON object to be run synchronously.
        /// </param>
        /// <returns> A JSON object result. </returns>
        private JObject GetResult(Task<JObject> asyncRequestTask)
        {
            return Task.Run(() => asyncRequestTask).GetAwaiter().GetResult();
        }
    }
}
