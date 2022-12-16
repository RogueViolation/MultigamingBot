using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Utilities
{
    public interface IHttpClientProvider
    {
        void SetupHttpClientHeaders(string name, string value);
        HttpClient ProvideClient();
    }
}
