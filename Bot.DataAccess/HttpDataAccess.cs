using Bot.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Bot.DataAccess
{
    public class HttpDataAccess : IHttpDataAccess
    {
        private readonly ILogger<HttpDataAccess> _logger;
        private readonly IHttpClientProvider _httpClientProvider;
        private readonly HttpClient _httpClient;
        public HttpDataAccess(ILogger<HttpDataAccess> logger, IHttpClientProvider httpClientProvider)
        {
            _logger = logger;
            _httpClientProvider = httpClientProvider;
            _httpClient = _httpClientProvider.ProvideClient();
        }
        public string HttpClientPost(string address, string request, Dictionary<string, string>? headers = null)
        {
            try
            {
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        content.Headers.Add(header.Key, header.Value);
                    }
                }
                var response = _httpClient.PostAsync(address, content).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occured while executing GET. {e.Message}");
            }

            return string.Empty;

        }

        public string HttpClientGet(string address, Dictionary<string, string>? headers = null)
        {
            try
            {
                var response = _httpClient.GetAsync(address).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occured while executing GET. {e.Message}");
            }
            return string.Empty;
        }

        public T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json) ?? default;
        }
    }
}
