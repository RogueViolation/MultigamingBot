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

namespace Bot.DataAccess
{
    public class HttpDataAccess : IHttpDataAccess
    {
        public readonly ILogger<HttpDataAccess> _logger;
        public readonly IHttpClientProvider _httpClientProvider;
        public readonly HttpClient _httpClient;
        public HttpDataAccess(ILogger<HttpDataAccess> logger, IHttpClientProvider httpClientProvider)
        {
            _logger = logger;
            _httpClientProvider = httpClientProvider;
            _httpClient = _httpClientProvider.ProvideClient();
        }
        public T HttpClientPostJson<T>(string address, string content, Dictionary<string, string>? headers = null)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    _httpClientProvider.SetupHttpClientHeaders(header.Key, header.Value);
                }
            }

            var response = _httpClient.PostAsync(address, new StringContent(content, Encoding.UTF8, "application/json"));

            var text = response.GetAwaiter().GetResult().Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<T>(text);
        }

        public T HttpClientGetJson<T>(string address, Dictionary<string, string>? headers = null)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    _httpClientProvider.SetupHttpClientHeaders(header.Key, header.Value);
                }
            }

            var response = _httpClient.GetAsync(address);

            var text = response.GetAwaiter().GetResult().Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}
