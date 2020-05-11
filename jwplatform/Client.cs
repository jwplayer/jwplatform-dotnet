using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace jwplatform
{
    /// <summary>
    /// Used to create a custom JW Platform HTTP Client.
    /// </summary>
    internal class Client
    {
        private readonly string apiKey;
        private readonly string apiSecret;

        private readonly HttpClient httpClient;
        private static readonly WebClient uploadClient = new WebClient();

        /// <summary>
        /// Constructor used to create Client containing an HttpClient used to execute JW Platform API requests.
        /// </summary>
        /// <param name="apiKey"> A JW Platform API Key. </param>
        /// <param name="apiSecret"> A JW Platform API Secret. </param>
        public Client(string apiKey, string apiSecret) : this(apiKey, apiSecret,
            new HttpClient { BaseAddress = new Uri("https://api.jwplatform.com/v1") })
        { }

        /// <summary>
        /// Constructor used for unit testing.
        /// </summary>
        /// <param name="apiKey"> A JW Platform API Key. </param>
        /// <param name="apiSecret"> A JW Platform API Secret. </param>
        /// <param name="httpClient"> An HttpClient. </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="apiKey" /> or <paramref name="apiSecret" /> is null.
        /// </exception>
        internal Client(string apiKey, string apiSecret, HttpClient httpClient)
        {
            if (apiKey == null || apiSecret == null)
                throw new ArgumentNullException("API Key and API Secret cannot be null");

            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Used to fulfill GET requests.
        /// </summary>
        /// <param name="requestPath"> A string representing the request route. </param>
        /// <param name="requestParams"> A Dictionary of string keys and values of the request parameters. </param>
        /// <returns> An HttpResponseMessage from the request. </returns>
        public async Task<HttpResponseMessage> GetAsync(string requestPath, Dictionary<string, string> requestParams)
        {
            var fullPath = requestPath + BuildQueryParams(requestParams);
            return await httpClient.GetAsync(fullPath);
        }

        /// <summary>
        /// Used to fulfill POST requests.
        /// </summary>
        /// <param name="requestPath"> A string representing the request route. </param>
        /// <param name="requestParams"> A Dictionary of string keys and values of the request parameters. </param>
        /// <param name="hasBodyParams">
        /// A boolean indicating if the <paramref name="requestParams" /> are body or query parameters.
        /// </param>
        /// <returns> An HttpResponseMessage from the request. </returns>
        public async Task<HttpResponseMessage> PostAsync(string requestPath, Dictionary<string, string> requestParams, bool hasBodyParams)
        {
            var fullPath = requestPath + (hasBodyParams ? "" : BuildQueryParams(requestParams));
            var content = hasBodyParams ? new FormUrlEncodedContent(BuildBodyParams(requestParams)) : null;

            return await httpClient.PostAsync(fullPath, content);
        }

        /// <summary>
        /// Used to fulfill Upload requests.
        /// </summary>
        /// <param name="uploadUrl"> A string representing the url to upload the file to. </param>
        /// <param name="filePath"> A string representing the local video file path. </param>
        /// <returns> A response string from the request. </returns>
        public async Task<string> UploadAsync(string uploadUrl, string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File does not exist");

            var response = await uploadClient.UploadFileTaskAsync(new Uri(uploadUrl), filePath);
            return Encoding.ASCII.GetString(response);
        }

        /// <summary>
        /// Alphabetically orders, encodes, and signs the <paramref name="requestParams" /> to be used
        /// as query parameters for a JW Platform API request.
        /// </summary>
        /// <param name="requestParams"> A Dictionary of string keys and values of the request parameters. </param>
        /// <returns> A string of query parameters. </returns>
        private string BuildQueryParams(Dictionary<string, string> requestParams)
        {
            var orderedParams = OrderParams(requestParams);
            var encodedParams = EncodeParams(orderedParams);
            var apiSignature = GenerateApiSignature(encodedParams);
            return "?" + encodedParams + "&api_signature=" + apiSignature;
        }

        /// <summary>
        /// Alphabetically orders, encodes, and signs the <paramref name="requestParams" /> to be used
        /// as body parameters for a JW Platform API request.
        /// </summary>
        /// <param name="requestParams"> A Dictionary of string keys and values of the request parameters. </param>
        /// <returns> A SortedDictionary with string keys and values of body parameters. </returns>
        private SortedDictionary<string, string> BuildBodyParams(Dictionary<string, string> requestParams)
        {
            var orderedParams = OrderParams(requestParams);
            var encodedParams = EncodeParams(orderedParams);
            var apiSignature = GenerateApiSignature(encodedParams);
            orderedParams.Add("api_signature", apiSignature);
            return orderedParams;
        }

        /// <summary>
        /// Alphabetically orders the <paramref name="requestParams" /> and adds the default parameters
        /// required for a JW Platform API request.
        /// </summary>
        /// <param name="requestParams"> A Dictionary of string keys and values of the request parameters. </param>
        /// <returns> A SortedDictionary of <paramref name="requestParams" />. </returns>
        private SortedDictionary<string, string> OrderParams(Dictionary<string, string> requestParams)
        {
            var orderedParams = requestParams == null
                ? new SortedDictionary<string, string>()
                : new SortedDictionary<string, string>(requestParams);

            orderedParams.Add("api_key", apiKey);
            orderedParams.Add("api_format", "json");
            orderedParams.Add("api_nonce", ClientUtils.GenerateNonce());
            orderedParams.Add("api_timestamp", ClientUtils.GetCurrentTimeInSeconds().ToString());

            return orderedParams;
        }

        /// <summary>
        /// Encodes the <paramref name="orderedParams" /> to be compliant with the JW Platform API.
        /// </summary>
        /// <param name="orderedParams">
        /// A SortedDictionary of string keys and values of request parameters.
        /// </param>
        /// <returns> An string of the encoded query parameters. </returns>
        private string EncodeParams(SortedDictionary<string, string> orderedParams)
        {
            var encodedParams = new StringBuilder();
            foreach (var param in orderedParams.Keys)
            {
                if (encodedParams.Length != 0)
                    encodedParams.Append("&");

                var encodedKey = ClientUtils.Encode(param);
                var encodedValue = ClientUtils.Encode(orderedParams[param]);
                encodedParams.Append(string.Format("{0}={1}", encodedKey, encodedValue));
            }
            return encodedParams.ToString();
        }

        /// <summary>
        /// Generates a secure hash from the encoded request parameters to be used as the API Signature for the
        /// JW Platform API request.
        /// </summary>
        /// <param name="encodedParams"> A string of the encoded request parameters. </param>
        /// <returns> A string of the API Signature parameter. </returns>
        private string GenerateApiSignature(string encodedParams)
        {
            encodedParams += apiSecret;
            return ClientUtils.GetSha1Hex(encodedParams);
        }
    }
}
