using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bot.Utilities
{
    public class HttpClientProvider : IHttpClientProvider
    {
        public readonly HttpClient _httpClient;

        public HttpClientProvider() 
        {
            _httpClient = new HttpClient(); 
        }

        public void SetupHttpClientHeaders(string name, string value)
        {
            _httpClient.DefaultRequestHeaders.Add(name,value);
        }

        public HttpClient ProvideClient()
        {
            return _httpClient;
        }
    }
}
