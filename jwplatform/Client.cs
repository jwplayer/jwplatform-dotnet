using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace jwplatform
{
    internal class Client
    {
        private readonly string apiKey;
        private readonly string apiSecret;

        internal readonly HttpClient httpClient;
        private static readonly WebClient uploadClient = new WebClient();

        public Client(string apiKey, string apiSecret) : this(apiKey, apiSecret,
            new HttpClient { BaseAddress = new Uri("https://api.jwplatform.com/v1") })
        { }

        internal Client(string apiKey, string apiSecret, HttpClient httpClient)
        {
            if (apiKey == null || apiSecret == null)
                throw new ArgumentNullException("API Key and API Secret cannot be null");

            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
            this.httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetAsync(string path, Dictionary<string, string> requestParams)
        {
            var fullUri = path + BuildParams(requestParams);
            return await httpClient.GetAsync(fullUri);
        }

        public async Task<HttpResponseMessage> PostAsync(string path, Dictionary<string, string> requestParams, bool hasBodyParams)
        {
            var content = hasBodyParams ? new StringContent(JsonConvert.SerializeObject(requestParams)) : null;
            var fullUri = path + BuildParams(hasBodyParams ? null : requestParams);

            return await httpClient.PostAsync(fullUri, content);
        }

        public async Task<string> UploadAsync(string uploadUrl, string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File does not exist");

            var response = await uploadClient.UploadFileTaskAsync(new Uri(uploadUrl), filePath);
            return Encoding.ASCII.GetString(response);
        }

        private string BuildParams(Dictionary<string, string> requestParams)
        {
            var orderedParams = OrderParams(requestParams);
            var encodedParams = EncodeParams(orderedParams);
            var apiSignature = GenerateApiSignatureParam(encodedParams);

            var signedParams = "?" + encodedParams + "&" + apiSignature;
            return signedParams;
        }

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

        private string GenerateApiSignatureParam(string encodedParams)
        {
            encodedParams += apiSecret;
            return string.Format("api_signature={0}", ClientUtils.GetSha1Hex(encodedParams.ToString()));
        }
    }
}
