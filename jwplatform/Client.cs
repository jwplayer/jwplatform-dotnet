using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace jwplatform
{
    public class Client
    {
        private readonly string apiKey;
        private readonly string apiSecret;

        private readonly HttpClient httpClient;

        public Client(string apiKey, string apiSecret) {
            if (apiKey == null || apiSecret == null)
                throw new ArgumentNullException("API Key and API Secret cannot be null");

            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
            this.httpClient = new HttpClient {
                BaseAddress = new Uri("https://api.jwplatform.com/v1"),
            };
        }

        public async Task<HttpResponseMessage> GetAsync(string path, Dictionary<string, string> requestParams) {
            var fullUri = path + BuildParams(requestParams);
            try {
                return await httpClient.GetAsync(fullUri);
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string path, Dictionary<string, string> requestParams, bool hasBodyParams) {
            var array = requestParams.ToArray();
            var content = hasBodyParams ? new StringContent(JsonConvert.SerializeObject(requestParams)) : null;
            var fullUri = path + BuildParams(hasBodyParams ? null : requestParams);

            return await httpClient.PostAsync(fullUri, content);
        }

        internal string Upload(string uploadUrl, string filePath) {
            if (!File.Exists(filePath))
                throw new ArgumentException("File does not exist");

            using (var webClient = new WebClient()) {
                try {
                    var response = webClient.UploadFile(new Uri(uploadUrl), filePath);
                    return System.Text.Encoding.ASCII.GetString(response);
                }
                catch(Exception ex) {
                    throw ex;
                }
            }
        }

        private string BuildParams(Dictionary<string, string> requestParams) {
            var orderedParams = OrderParams(requestParams);
            var encodedParams = EncodeParams(orderedParams);
            var apiSignature = GenerateApiSignatureParam(encodedParams);

            var signedParams = "?" + encodedParams + "&" + apiSignature;
            return signedParams;
        }

        private SortedDictionary<string, string> OrderParams(Dictionary<string, string> requestParams) {
            var orderedParams = requestParams == null 
                ? new SortedDictionary<string, string>() 
                : new SortedDictionary<string, string>(requestParams);

            orderedParams.Add("api_key", this.apiKey);
            orderedParams.Add("api_format", "json");
            orderedParams.Add("api_nonce", ClientUtils.GenerateNonce());
            orderedParams.Add("api_timestamp", ClientUtils.GetCurrentTimeInSeconds().ToString());

            return orderedParams;
        }

        private string EncodeParams(SortedDictionary<string, string> orderedParams) {
            var encodedParams = new StringBuilder();
            foreach (var param in orderedParams.Keys) {
                if (encodedParams.Length != 0)
                    encodedParams.Append("&");

                var encodedKey = ClientUtils.EncodeString(param);
                var encodedValue = ClientUtils.EncodeString(orderedParams[param]);
                encodedParams.Append(string.Format("{0}={1}", encodedKey, encodedValue));
            }
            return encodedParams.ToString();
        }

        private string GenerateApiSignatureParam(string encodedParams) {
            encodedParams += this.apiSecret;
            return string.Format("api_signature={0}", ClientUtils.GetSha1Hex(encodedParams.ToString()));
        }
    }
}
